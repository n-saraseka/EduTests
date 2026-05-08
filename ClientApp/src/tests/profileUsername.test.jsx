/** @vitest-environment jsdom */
import { render, screen, cleanup } from '@testing-library/react';
import { describe, it, expect, vi, afterEach } from 'vitest';
import ProfileUsername from '../profile/profileUsername.jsx';

vi.mock('../buttons/reportButton.jsx', () => ({ default: () => <button>Report</button> }));
vi.mock('../buttons/banButton.jsx', () => ({ default: () => <button>Ban</button> }));
vi.mock('../buttons/deleteButton.jsx', () => ({ default: () => <button>Delete</button> }));
vi.mock('../buttons/promoteButton.jsx', () => ({ default: () => <button>Promote</button> }));
vi.mock('../bannedLabel.jsx', () => ({ default: () => <div>Banned label</div> }));

describe('ProfileUsername Unit Tests', () => {
    const mockUser = { id: 10, username: 'TestUser' };

    afterEach(() => {
        cleanup();
    });

    it('must show username', () => {
        render(<ProfileUsername user={mockUser} currentUserId={1} currentUserGroup="User" isBanned={false} />);
        expect(screen.getByText('TestUser')).toBeTruthy();
    });

    it('regular user shouldnt see admin buttons', () => {
        render(<ProfileUsername user={mockUser} currentUserId={1} currentUserGroup="User" isBanned={false} />);

        expect(screen.getByText('Report')).toBeTruthy();
        expect(screen.queryByText('Ban')).toBeNull();
        expect(screen.queryByText('Delete')).toBeNull();
    });

    it('admin should see all buttons', () => {
        render(<ProfileUsername
            user={mockUser}
            currentUserId={1}
            currentUserGroup="Administrator"
            isBanned={false}
        />);

        expect(screen.getByText('Ban')).toBeTruthy();
        expect(screen.getByText('Promote')).toBeTruthy();
        expect(screen.getByText('Delete')).toBeTruthy();
    });

    it('if user is banned show only bannedlabel', () => {
        render(<ProfileUsername user={mockUser} currentUserId={1} currentUserGroup="Administrator" isBanned={true} />);

        expect(screen.getByText('Banned label')).toBeTruthy();
        expect(screen.queryByText('Report')).toBeNull();
        expect(screen.queryByText('Ban')).toBeNull();
    });

    it('user shouldnt see the self report button', () => {
        render(<ProfileUsername user={mockUser} currentUserId={10} currentUserGroup="User" isBanned={false} />);

        expect(screen.queryByText('Report')).toBeNull();
    });
});