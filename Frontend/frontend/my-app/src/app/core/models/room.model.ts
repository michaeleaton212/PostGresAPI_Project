export interface Room {
  id: number;
  name: string;
  type: string;
  numberOfBeds?: number;
  numberOfChairs?: number;
  image?: string;   // optional main image (for backward compatibility)
  images?: string[];   // new property for multiple images
}

export interface Bedroom extends Room {
  numberOfBeds: number;
}

export interface Meetingroom extends Room {
  numberOfChairs: number;
}
