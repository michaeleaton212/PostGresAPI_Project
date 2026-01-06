export interface Review {
  id: number;
  userId: number;
  userName: string;
  title: string;
  content: string;
  rating: number;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateReviewDto {
  title: string;
  content: string;
  rating: number;
}

export interface UpdateReviewDto {
  title: string;
  content: string;
  rating: number;
}
