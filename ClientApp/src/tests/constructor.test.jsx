/** @vitest-environment jsdom */
import {describe, it, expect, vi, afterEach} from 'vitest';
import Constructor, {createTest, CreateOrUpdateTestCommand} from "../constructor/constructor.jsx";
import {cleanup, render, screen} from "@testing-library/react";

vi.mock('../constructor/testTab.jsx', () => ({ default: () => <div>Test settings</div> }));
vi.mock('../constructor/resultTab.jsx', () => ({ default: () => <div>Result settings</div> }));
vi.mock('../constructor/testAside.jsx', () => ({ default: () => <div>Aside</div> }));

describe('Constructor Unit Tests', () => {
    afterEach(() => {
        cleanup();
    });
    
    const mockUser = { id: 1 };

    it('shows TestTab as default', () => {
        render(<Constructor user={mockUser} baseTest={null} />);

        expect(screen.getByText('Test settings')).toBeTruthy();
        expect(screen.queryByText('Result settings')).toBeNull();
    });

    it('should initialize data correctly', () => {
        const command = new CreateOrUpdateTestCommand('Название', 'Описание', [], [], 1, 60, [], 'pass', 0, true);

        expect(command.name).toBe('Название');
        expect(command.timeLimit).toBe(60);
        expect(command.showCorrectAnswers).toBe(true);
    });

    it('should create POST request with correct headers', async () => {
        fetch = vi.fn().mockResolvedValue({ ok: true });
        const command = { name: 'Test' };

        await createTest(command);

        expect(fetch).toHaveBeenCalledWith('/api/tests', expect.objectContaining({
            method: 'POST',
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(command)
        }));
    });
});