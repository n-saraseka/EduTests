using EduTests.ApiObjects;
using EduTests.Database.Entities;

namespace EduTests.Services;

public interface IEntityToDtoService
{
    Task<ApiTest> TestEntityToDtoAsync(Test entity, CancellationToken cancellationToken);
    ApiTest TestEntityToDto(Test entity);
    ApiRating RatingEntityToDto(UserRating entity);
    ApiComment CommentEntityToDto(Comment entity);
    ApiQuestion QuestionEntityToDto(Question entity);
    ApiCompletion CompletionEntityToDto(TestCompletion entity, List<UserAnswer>? userAnswers,
        List<Question>? questions);
    ApiAnswer AnswerEntityToDto(UserAnswer entity);
    ApiUser UserEntityToDto(User entity);
    ApiBan BanEntityToDto(BannedUser entity);
    ApiTag TagEntityToDto(Tag tag);
    ApiReport ReportEntityToDto(Report entity);
    ApiTestResult TestResultEntityToDto(TestResult entity);
}