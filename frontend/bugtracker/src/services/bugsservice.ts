import { Requester } from "./requester";

export type Bug = {
    id: number;
    title: string;
    description: string;
    status: 'Open' | 'In Progress' | 'Closed';
    assignedTo: string | undefined;
};

export class BugService extends Requester {
    private readonly url: string;

    public constructor() {
        super();

        this.url = `${process.env.REACT_APP_BUG_SERVICE_URL}/bugs`;
    }

    public async getBugs(): Promise<Bug[]> {
        const response = await this.sendRequest('Get', this.url);
        return await response.json();
    }

    public async getBug(id: number): Promise<Bug> {
        const response = await this.sendRequest('Get', `${this.url}/${id}`);
        return await response.json();
    }

    public async createBug(bug: Bug): Promise<void> {
        await this.sendRequest('Post', this.url, { ...bug, id: undefined });
    }

    public async updateBug(bug: Bug): Promise<void> {
        await this.sendRequest('Put', `${this.url}/${bug.id}`, bug);
    }

    public async deleteBug(id: number): Promise<void> {
        await this.sendRequest('Delete', `${this.url}/${id}`);
    }

    public getStatuses(): string[] {
        return ['Open', 'In Progress', 'Closed'];
    }

}