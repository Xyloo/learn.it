export interface GroupDetails {
  groupId: number;
  name: string;
  creator: {
    username: string;
    avatar: string | null;
  };
  users: {
    username: string;
    avatar: string | null;
  }[];
  studySets: {
    studySetId: number;
    name: string;
    description: string;
    flashcardsCount: number;
  }[];
}
