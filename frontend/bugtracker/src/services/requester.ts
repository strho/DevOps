
export type HttpMethod = 'Get' | 'Post' | 'Put' | 'Delete';

/** Allows to send authenticated requests to Azure DevOps instance. */
export abstract class Requester {

    /**
     * Sends an authenticated request to Azure DevOps and validates the response. If the response is not successful, throws an {@link AdoError}.
     * @param method Request method
     * @param url Request URL
     * @param body Request body
     */
    protected async sendRequest(method: HttpMethod, url: string, body?: object): Promise<Response> {

        const request: RequestInit = {
            headers: {
                'Content-Type': 'application/json',
            },
            method,
            body: typeof body !== 'undefined' ? JSON.stringify(body) : undefined
        };

        const response = await fetch(url, request);
        if (!response.ok) {
            throw new Error(`Request '${method} ${url}' failed with status ${response.status}`);
        }

        return response;
    }
}