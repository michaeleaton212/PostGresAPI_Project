// comments-page.ts
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { FooterComponent } from '../../components/core/footer/footer';
import { ReviewService } from '../../core/review.service';
import { Review, CreateReviewDto } from '../../core/models/review.model';

@Component({
  selector: 'app-comments-page',
  standalone: true,
  imports: [CommonModule, FormsModule, FooterComponent],
  templateUrl: './review-page.html',
  styleUrl: './review-page.scss',
})
export class CommentsPage implements OnInit {
  private reviewService = inject(ReviewService);
  private route = inject(ActivatedRoute);

  reviews: Review[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  hoverRating = 0;
  showWriteReview = false;

  averageRating = 0;

  newReview: CreateReviewDto = {
    title: '',
    content: '',
    rating: 5,
  };

  get isLoggedIn(): boolean {
    return !!sessionStorage.getItem('userId');
  }

  ngOnInit(): void {
    this.loadReviews();

    this.route.queryParams.subscribe(params => {
      this.showWriteReview = params['write'] === 'true';
    });
  }

  loadReviews(): void {
    this.isLoading = true;
    this.reviewService.getAllReviews().subscribe({
      next: (reviews) => {
        this.reviews = reviews.sort((a, b) =>
          new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        );
        this.recalcAverage();
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Failed to load reviews.';
      }
    });
  }

  private recalcAverage(): void {
    if (!this.reviews.length) {
      this.averageRating = 0;
      return;
    }
    const sum = this.reviews.reduce((acc, r) => acc + (r.rating ?? 0), 0);
    this.averageRating = sum / this.reviews.length;
  }

  getStarFillPercent(starIndex: number): number {
    // starIndex: 1..5
    const value = this.averageRating - (starIndex - 1);
    if (value >= 1) return 100;
    if (value <= 0) return 0;
    return Math.round(value * 100); // 0.5 => 50%
  }

  setRating(value: number): void {
    this.newReview.rating = value;
  }

  setHover(value: number): void {
    this.hoverRating = value;
  }

  clearHover(): void {
    this.hoverRating = 0;
  }

  submitReview(): void {
    if (!this.newReview.title.trim() || !this.newReview.content.trim()) return;

    this.isLoading = true;
    this.reviewService.createReview(this.newReview).subscribe({
      next: (review) => {
        this.successMessage = 'Review submitted successfully!';
        this.reviews.unshift(review);
        this.recalcAverage();

        this.newReview = { title: '', content: '', rating: 5 };
        this.isLoading = false;

        setTimeout(() => {
          this.successMessage = '';
          this.showWriteReview = false;
        }, 2000);
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Error submitting review.';
      }
    });
  }

  cancelWriteReview(): void {
    this.showWriteReview = false;
    this.newReview = { title: '', content: '', rating: 5 };
    this.errorMessage = '';
    this.successMessage = '';
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('de-DE', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }
}
