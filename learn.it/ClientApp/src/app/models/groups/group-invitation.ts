export interface GroupInvitation {
  userId: number;
  creatorId: number;
  group: {
    groupId: number;
    name: string | null;
  };
}
