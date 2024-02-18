import { Requester } from "./requester";

export type User = {
    id: number;
    name: string;
    email: string;
    department: string;
};

export class UserService extends Requester {

    private readonly url: string;

    public constructor() {
        super();

        this.url = 'http://localhost:8081/users';
    }

    public async getUsers(): Promise<User[]> {
        const response = await this.sendRequest('Get', this.url);
        return await response.json();
    }

    public async getUser(id: number): Promise<User> {
        const response = await this.sendRequest('Get', `${this.url}/${id}`);
        return await response.json();
    }

    public async getUserByName(name: string): Promise<User> {
        const response = await this.sendRequest('Get', `${this.url}/filter/${name}`);
        return await response.json();
    }

    public async createUser(user: User): Promise<void> {
        await this.sendRequest('Post', this.url, { ...user, id: undefined });
    }

    public async updateUser(user: User): Promise<void> {
        await this.sendRequest('Put', `${this.url}/${user.id}`, user);
    }

    public async deleteUser(id: number): Promise<void> {
        await this.sendRequest('Delete', `${this.url}/${id}`);
    }

    public getDepartments(): string[] {
        return ['Engineering', 'Sales', 'Marketing', 'HR', 'Finance'];
    }
}