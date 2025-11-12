export interface Room {
  id: number;
  name: string;
  type: string;
}

export interface Bedroom extends Room {
  numberOfBeds?: number;
}

export interface Meetingroom extends Room {
  numberOfChairs?: number;
}
