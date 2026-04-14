using EduTests.ApiObjects;
using EduTests.Commands.ReportCommands;
using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Services.Questions;

namespace EduTests.Services;

public class EntityToDtoService(IUserRatingRepository ratingRepository,
    ITestCompletionRepository testCompletionRepository,
    IAnswerVerifierService answerVerifierService) : IEntityToDtoService
{
    /// <summary>
    /// Map <see cref="Test"/> entity to <see cref="ApiTest"/> DTO
    /// </summary>
    /// <param name="entity">The <see cref="Test"/> entity</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>The <see cref="ApiTest"/> DTO</returns>
    public async Task<ApiTest> TestEntityToDtoAsync(Test entity, CancellationToken cancellationToken)
    {
        var ratings = await ratingRepository.GetTestRatingAsync(entity.Id, cancellationToken);
        var completions = await testCompletionRepository.GetTestCompletionCountAsync(entity.Id, cancellationToken);
        var tags = entity.Tags.Select(t => t.Name).ToList();
        var questions = entity.Questions.Select(QuestionEntityToDto).ToList();
        
        var testToReturn = new ApiTest
        {
            Id = entity.Id,
            Name = entity.Name,
            User = UserEntityToDto(entity.User),
            Description = entity.Description,
            ThumbnailUrl = entity.ThumbnailUrl,
            Rating = ratings,
            CompletionCount = completions,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            AttemptLimit = entity.AttemptLimit,
            TimeLimit = entity.TimeLimit,
            Tags = tags,
            Questions = questions,
            Results = entity.Results.Select(TestResultEntityToDto).ToList(),
            DefaultResult = entity.DefaultResult
        };
        
        return testToReturn;
    }

    /// <summary>
    /// Map <see cref="Test"/> entity to simplified <see cref="ApiTest"/> DTO
    /// This one doesn't include the rating, the completions or tags.
    /// </summary>
    /// <param name="entity">The <see cref="Test"/> entity</param>
    /// <returns>The simplified <see cref="ApiTest"/> DTO</returns>
    public ApiTest TestEntityToDto(Test entity)
    {
        var testToReturn = new ApiTest
        {
            Id = entity.Id,
            Name = entity.Name,
            User = UserEntityToDto(entity.User),
            Description = entity.Description,
            ThumbnailUrl = entity.ThumbnailUrl,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            AttemptLimit = entity.AttemptLimit,
            TimeLimit = entity.TimeLimit,
        };

        return testToReturn;
    }
    
    /// <summary>
    /// Map <see cref="UserRating"/> entity to <see cref="ApiRating"/> DTO
    /// </summary>
    /// <param name="entity">The <see cref="UserRating"/> entity</param>
    /// <returns><see cref="ApiRating"/> DTO</returns>
    public ApiRating RatingEntityToDto(UserRating entity)
    {
        var ratingToReturn = new ApiRating
        {
            TestId = entity.TestId,
            UserId = entity.UserId,
            IsPositive = entity.IsPositive
        };
        
        return ratingToReturn;
    }
    
    /// <summary>
    /// Map <see cref="Comment"/> entity to <see cref="ApiComment"/> DTO
    /// </summary>
    /// <param name="entity">The <see cref="Comment"/> entity</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>The <see cref="ApiComment"/> DTO</returns>
    /// <exception cref="NullReferenceException">In case <see cref="Comment.UserProfileId"/> and <see cref="Comment.TestId"/> both are null,
    /// or if the <see cref="User"/> associated with the <see cref="Comment.CommenterId"/> is null</exception>
    public ApiComment CommentEntityToDto(Comment entity)
    {
        var entityType = (entity.UserProfileId != null) ? CommentEntityType.UserProfile : CommentEntityType.Test;
        var entityId = entity.UserProfileId ?? entity.TestId;
        
        if (entityId is null)
            throw new NullReferenceException(nameof(entityId));
        
        var apiUser = UserEntityToDto(entity.Commenter);

        var commentToReturn = new ApiComment
        {
            Id = entity.Id,
            UserId = entity.CommenterId,
            User = apiUser,
            EntityType = entityType,
            EntityId = (int)entityId,
            Content = entity.Content,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };

        return commentToReturn;
    }
    
    /// <summary>
    /// Map a <see cref="Question"/> entity to <see cref="ApiQuestion"/> DTO
    /// </summary>
    /// <param name="entity">The <see cref="Question"/> entity</param>
    /// <returns>The <see cref="ApiQuestion"/> DTO</returns>
    public ApiQuestion QuestionEntityToDto(Question entity)
    {
        var questionToReturn = new ApiQuestion
        {
            Id = entity.Id,
            TestId = entity.TestId,
            OrderIndex = entity.OrderIndex,
            Type = entity.Type,
            Description = entity.Description,
            Data = entity.Data,
            CorrectData = entity.CorrectData,
        };
        
        return questionToReturn;
    }
    
    /// <summary>
    /// Convert <see cref="TestCompletion"/> entity to <see cref="ApiCompletion"/> DTO
    /// </summary>
    /// <param name="entity">The <see cref="TestCompletion"/> entity</param>
    /// <param name="userAnswers">List of <see cref="UserAnswer"/>s (if the completion has finished)</param>
    /// <param name="questions">List of <see cref="Question"/>s (if the completion has finished)</param>
    /// <returns>The <see cref="ApiCompletion"/> DTO</returns>
    /// <exception cref="ArgumentNullException">If there's no corresponding <see cref="Question"/> for a <see cref="UserAnswer"/></exception>
    public ApiCompletion CompletionEntityToDto(TestCompletion entity, List<UserAnswer>? userAnswers, List<Question>? questions)
    {
        var userId = entity.UserId;
        var anonymousId = entity.AnonymousUserId;
        
        if (userId is null && anonymousId is null)
            throw new ArgumentException($"Neither {nameof(entity.UserId)} or {nameof(entity.AnonymousUserId)} are not null");
        
        var completionToReturn = new ApiCompletion
        {
            Id = entity.Id,
            TestId = entity.TestId,
            UserId = userId,
            StartedAt = entity.StartedAt,
            CompletedAt = entity.CompletedAt,
        };
        
        if (entity.CompletedAt is not null)
        {
            var questionCount = questions.Count;
            var correctAnswers = 0;
            var correctPercentage = 0.0;

            foreach (var answer in userAnswers)
            {
                var correspondingQuestion = questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                if (correspondingQuestion is null)
                    throw new ArgumentNullException(nameof(correspondingQuestion));
                if (answerVerifierService.Verify(answer, correspondingQuestion, correspondingQuestion.Type))
                    correctAnswers++;
            }
            
            completionToReturn.CorrectAnswers = correctAnswers;
            correctPercentage = correctAnswers * 100.0 / questionCount;
            completionToReturn.CompletionPercentage = Math.Round(correctPercentage, 2);
        }

        return completionToReturn;
    }
    
    /// <summary>
    /// Map <see cref="UserAnswer"/> entity to <see cref="UserAnswer"/> DTO
    /// </summary>
    /// <param name="entity">The <see cref="UserAnswer"/> entity</param>
    /// <returns>The <see cref="ApiAnswer"/> DTO</returns>
    public ApiAnswer AnswerEntityToDto(UserAnswer entity)
    {
        var answerToReturn = new ApiAnswer
        {
            Id = entity.Id,
            TestCompletionId = entity.TestCompletionId,
            QuestionId = entity.QuestionId,
            Answer = entity.Answers
        };
        
        return answerToReturn;
    }
    
    /// <summary>
    /// Map <see cref="User"/> entity to <see cref="ApiUser"/> DTO
    /// </summary>
    /// <param name="entity">The <see cref="User"/> entity</param>
    /// <returns>The <see cref="ApiUser"/> DTO</returns>
    public ApiUser UserEntityToDto(User entity)
    {
        var apiUser = new ApiUser
        {
            Id = entity.Id,
            Username = entity.Username,
            AvatarUrl = entity.AvatarUrl,
            Description = entity.Description,
            RegistrationDate = entity.RegistrationDate,
            Group = entity.Group
        };
        
        return apiUser;
    }
    
    /// <summary>
    /// Map <see cref="BannedUser"/> entity to <see cref="ApiBan"/> DTO
    /// </summary>
    /// <param name="entity">The <see cref="BannedUser"/> entity</param>
    /// <returns>The <see cref="ApiBan"/> DTO</returns>
    public ApiBan BanEntityToDto(BannedUser entity)
    {
        var apiBan = new ApiBan
        {
            Id = entity.Id,
            BannedUserId = entity.UserBannedId,
            BannedUser = UserEntityToDto(entity.UserBanned),
            BannedByUser = UserEntityToDto(entity.BannedBy),
            BannedByUserId = entity.BannedById,
            BanReason = entity.BanReason,
            BanDate = entity.DateBanned,
            UnbanDate = entity.DateUnbanned
        };
        
        return apiBan;
    }
    
    /// <summary>
    /// Map <see cref="Tag"/> entity to <see cref="ApiTag"/> DTO
    /// </summary>
    /// <param name="tag">The <see cref="Tag"/> entity</param>
    /// <returns>The <see cref="ApiTag"/> DTO</returns>
    public ApiTag TagEntityToDto(Tag tag)
    {
        var apiTag = new ApiTag
        {
            Id = tag.Id,
            Name = tag.Name
        };
        
        return apiTag;
    }
    
    /// <summary>
    /// Map <see cref="Report"/> entity to <see cref="ApiReport"/> DTO
    /// </summary>
    /// <param name="entity">The <see cref="Report"/> entity</param>
    /// <returns>The <see cref="ApiReport"/> DTO</returns>
    public ApiReport ReportEntityToDto(Report entity)
    {
        var apiTest = entity.Test != null ? TestEntityToDto(entity.Test) : null;
        var apiComment = entity.Comment != null ? CommentEntityToDto(entity.Comment) : null;
        var apiUser = entity.User != null ? UserEntityToDto(entity.User) : null;
        
        var apiReport = new ApiReport
        {
            Id = entity.Id,
            ReportedTest = apiTest,
            ReportedComment = apiComment,
            ReportedUser = apiUser,
            ReportText = entity.Text,
            DateReported = entity.DateTime,
            ReportStatus = entity.ReportStatus
        };
        
        return apiReport;
    }

    public ApiTestResult TestResultEntityToDto(TestResult entity)
    {
        var apiResult = new ApiTestResult
        {
            PercentageThreshold = entity.PercentageThreshold,
            Result = entity.Result
        };
        
        return apiResult;
    }
}