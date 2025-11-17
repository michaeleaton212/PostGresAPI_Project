export interface Room {
  id: number;
  name: string;
  type: string;
  numberOfBeds?: number | null;
  numberOfChairs?: number | null;
}

export interface Bedroom extends Room {
  numberOfBeds: number;
}

export interface Meetingroom extends Room {
  numberOfChairs: number;
}
