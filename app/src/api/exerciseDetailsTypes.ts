export interface ExerciseDetail {
  id: string;
  owner: string;
  creatorId: string;
  name: string;
  description: string;
  imageUrl: string | null;
  isPublic: boolean;
  muscleGroups: string;
}

export interface PaginatedResponse<T> {
  totalPages: number;
  currentPage: number;
  pageSize: number;
  totalItems: number;
  lastPage: boolean;
  items: T[];
}
