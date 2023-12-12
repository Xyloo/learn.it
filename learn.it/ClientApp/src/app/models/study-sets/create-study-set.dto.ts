export interface CreateStudySetDto {
  name: string;
  description: string;
  visibility: number;
  groupId?: number | null;
}
