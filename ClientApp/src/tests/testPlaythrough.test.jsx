/** @vitest-environment jsdom */
import {render, screen, fireEvent, waitFor, cleanup} from '@testing-library/react';
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import TestPlaythrough from '../test/testPlaythrough.jsx';

vi.mock('./playthroughQuestion.jsx', () => ({
    default: ({ question }) => <div data-testid="question-display">{question.text}</div>
}));

describe('TestPlaythrough Unit Tests', () => {
    const mockQuestions = [
        { id: 101, orderIndex: 1, text: 'First question', type: 1, data: { options: ["test"]}},
        { id: 102, orderIndex: 2, text: 'Second question', type: 1, data: { options: ["test"]} }
    ];

    const mockCompletion = { id: 1, testId: 50 };
    const mockTest = { id: 50, name: 'Name' };

    const defaultProps = {
        baseQuestions: mockQuestions,
        baseAnswers: [],
        baseLastUnanswered: 1,
        baseTest: mockTest,
        completion: mockCompletion
    };

    beforeEach(() => {
        vi.clearAllMocks();
        fetch = vi.fn().mockResolvedValue({ ok: true, json: () => Promise.resolve([]) });
    });
    
    afterEach(() => {
        cleanup();
    });

    it('should show first question', () => {
        render(<TestPlaythrough {...defaultProps} />);

        expect(screen.getByText(/Вопрос 1 из 2/i)).toBeTruthy();
    });

    it('should change the question after pressing continue', async () => {
        render(<TestPlaythrough {...defaultProps} />);

        const nextBtn = screen.getByText(/Продолжить/i);
        fireEvent.click(nextBtn);

        await waitFor(() => {
            expect(screen.getByText(/Вопрос 2 из 2/i)).toBeTruthy();
        });
    });

    it('should calculate the width of progress bar', () => {
        const { container } = render(<TestPlaythrough {...defaultProps} />);

        const progressBar = container.querySelector('#progress-bar');
        expect(progressBar.style.background).toContain('50%');
    });

    it('should change the button text on last question', () => {
        render(<TestPlaythrough {...defaultProps} baseLastUnanswered={2} />);

        expect(screen.getByText(/Завершить тест/i)).toBeTruthy();
    });
});