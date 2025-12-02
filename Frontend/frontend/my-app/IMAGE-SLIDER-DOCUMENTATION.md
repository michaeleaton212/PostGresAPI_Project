# Multi-Image Slider Documentation

## Overview
The bedroom preview page now supports displaying multiple images with a slider interface.

## Features Implemented

### 1. Room Model Update
- Added `images?: string[]` property to the `Room` interface
- Maintains backward compatibility with the existing `image?: string` property

### 2. Image Slider Component
The bedroom preview page now includes:
- **Navigation buttons**: Previous (‹) and Next (›) buttons to cycle through images
- **Image counter**: Shows current image position (e.g., "2 / 5")
- **Thumbnail gallery**: Small preview images below the main image
- **Click-to-select**: Click any thumbnail to jump to that image

### 3. Responsive Design
- Desktop: Full-sized controls and thumbnails
- Mobile: Smaller controls optimized for touch

## Usage

### Backend Integration
To enable multiple images, your API should return room data in this format:

```json
{
  "id": 1,
  "name": "Deluxe Bedroom",
  "type": "Bedroom",
  "numberOfBeds": 2,
  "image": "/images/bedroom-main.jpg",  // optional, for backward compatibility
  "images": [      // new property
    "/images/bedroom-1.jpg",
  "/images/bedroom-2.jpg",
    "/images/bedroom-3.jpg"
  ]
}
```

### Behavior
1. **Single image**: If only `image` is provided or `images` array has 1 item, no slider controls appear
2. **Multiple images**: If `images` array has 2+ items, slider controls and thumbnails appear
3. **Fallback**: If no images are provided, defaults to 'grey.png'

### Navigation Methods
- **Next/Previous buttons**: Cycle through images
- **Thumbnail click**: Jump directly to a specific image
- **Automatic wrapping**: Cycling past the last image returns to the first

## Styling Classes

### Main Elements
- `.image-room` - Container for the image and controls
- `.room-card-image` - Main display image
- `.image-controls` - Container for navigation buttons and counter
- `.thumbnails` - Thumbnail gallery container

### Control States
- `.nav-btn` - Navigation button (prev/next)
- `.counter` - Image position indicator
- `.thumbnails img.active` - Currently selected thumbnail

## Customization

### Modify Thumbnail Size
Edit in `bedroom-preview-page.component.scss`:
```scss
.thumbnails img {
  width: 80px;   // change width
  height: 60px;  // change height
}
```

### Change Button Appearance
```scss
.image-controls .nav-btn {
  width: 40px;        // button size
  height: 40px;
  background: rgba(255, 255, 255, 0.9);  // background color
}
```

### Adjust Main Image Size
```scss
.room-card-image {
  width: 50rem;   // desktop width
  height: 30rem;  // desktop height
}
```

## Mobile Responsive Breakpoint
The design switches to mobile layout at `768px` width.

## Next Steps
To apply the same feature to the meeting room preview:
1. Copy the image slider methods from `bedroom-preview-page.component.ts`
2. Update the `meetingroom-preview-page.component.html` template
3. Copy the image slider styles to `meetingroom-preview-page.component.scss`
