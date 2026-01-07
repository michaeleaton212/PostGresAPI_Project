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

  allReviews: Review[] = [];
  displayedReviews: Review[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  hoverRating = 0;
  showWriteReview = false;

  averageRating = 0;

  // Pagination properties
  private readonly reviewsPerPage = 5;
  private currentPage = 1;
  hasMoreReviews = false;

  newReview: CreateReviewDto = {
    title: '',
    content: '',
    rating: 5,
  };

  get isLoggedIn(): boolean {
    return !!sessionStorage.getItem('userId');
  }

  get reviews(): Review[] {
    return this.displayedReviews;
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
        this.allReviews = reviews.sort((a, b) =>
          new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        );
        this.currentPage = 1;
        this.updateDisplayedReviews();
        this.recalcAverage();
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Failed to load reviews.';
      }
    });
  }

  private updateDisplayedReviews(): void {
    const startIndex = 0;
    const endIndex = this.currentPage * this.reviewsPerPage;
    this.displayedReviews = this.allReviews.slice(startIndex, endIndex);
    this.hasMoreReviews = endIndex < this.allReviews.length;
  }

  loadMoreReviews(): void {
    this.currentPage++;
    this.updateDisplayedReviews();
  }

  private recalcAverage(): void {
    if (!this.allReviews.length) {
      this.averageRating = 0;
      return;
    }
    const sum = this.allReviews.reduce((acc, r) => acc + (r.rating ?? 0), 0);
    this.averageRating = sum / this.allReviews.length;
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
        this.allReviews.unshift(review);
        this.currentPage = 1;
        this.updateDisplayedReviews();
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
