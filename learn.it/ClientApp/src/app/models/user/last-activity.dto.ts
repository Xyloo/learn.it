export interface LastActivityDto {
  id: number;
  name: string;
  description: string;
  visibility: number;
  creator: {
    username: string;
    avatar: string | null;
  }
}
