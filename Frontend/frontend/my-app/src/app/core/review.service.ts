import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Review, CreateReviewDto, UpdateReviewDto } from './models/review.model';

@Injectable({ providedIn: 'root' })
export class ReviewService {
  private api = inject(ApiService);

  /**
   * Get all reviews
   */
  getAllReviews(): Observable<Review[]> {
    return this.api.get<Review[]>('reviews');
  }

  /**
   * Get review by ID
   */
  getReviewById(id: number): Observable<Review> {
    return this.api.get<Review>(`reviews/${id}`);
  }

  /**
   * Get reviews by user ID
   */
  getReviewsByUserId(userId: number): Observable<Review[]> {
    return this.api.get<Review[]>(`reviews/user/${userId}`);
  }

  /**
   * Create a new review (requires authentication)
   */
  createReview(dto: CreateReviewDto): Observable<Review> {
    return this.api.post<Review>('reviews', dto);
  }

  /**
   * Update an existing review (requires authentication and ownership)
   */
  updateReview(id: number, dto: UpdateReviewDto): Observable<Review> {
    return this.api.put<Review>(`reviews/${id}`, dto);
  }

  /**
   * Delete a review (requires authentication and ownership)
   */
  deleteReview(id: number): Observable<void> {
    return this.api.delete<void>(`reviews/${id}`);
  }
}
