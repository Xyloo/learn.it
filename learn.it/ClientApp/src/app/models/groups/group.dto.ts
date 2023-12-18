export interface GroupDto {
  id: number;
  name: string;
  count: number;
  creator: {
    username: string;
    avatar: string | null
  };
}
