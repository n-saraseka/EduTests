/** @vitest-environment jsdom */
import {render, screen, fireEvent, waitFor, cleanup} from '@testing-library/react';
import {describe, it, expect, vi, beforeEach, afterEach} from 'vitest';
import ProfileTests, {getTests} from '../profile/profileTests.jsx';

vi.mock('../testList/testCard.jsx', () => ({ default: () => <div data-testid="test-card">Card</div> }));
vi.mock('../pagination.jsx', () => ({ default: () => <div data-testid="pagination">Pagination</div> }));

describe('ProfileTests Unit Tests', () => {
    const mockTests = [{ id: 1, name: 'Test 1' }, { id: 2, name: 'Test 2' }];
    const defaultProps = {
        baseTests: mockTests,
        basePages: 2,
        pageSize: 10,
        userId: 1
    };

    beforeEach(() => {
        vi.clearAllMocks();
        fetch = vi.fn();
    });
    
    afterEach(() => {
        cleanup();
    });

    it('must render base test list and pagination', () => {
        render(<ProfileTests {...defaultProps} />);

        const cards = screen.getAllByTestId('test-card');
        expect(cards.length).toBe(2);
        expect(screen.getByTestId('pagination')).toBeTruthy();
    });

    it('must show loading indicator on sort change', async () => {
        fetch.mockResolvedValueOnce({
            ok: true,
            json: () => Promise.resolve({ tests: [], pages: 1 })
        });

        render(<ProfileTests {...defaultProps} />);

        const select = screen.getByLabelText(/Сортировка:/i);
        
        fireEvent.change(select, { target: { value: '1' } });
        
        const loader = screen.getByAltText('Загрузка контента');
        expect(loader).toBeTruthy();
        
        await waitFor(() => {
            expect(screen.queryByAltText('Загрузка контента')).toBeNull();
        });
    });

    it('getTests should form correct URL', () => {
        getTests(1, 10, 5, 2);

        expect(fetch).toHaveBeenCalledWith(
            expect.stringContaining('/api/tests?page=1&amountPerPage=10&userId=5&sort=2')
        );
    });
});