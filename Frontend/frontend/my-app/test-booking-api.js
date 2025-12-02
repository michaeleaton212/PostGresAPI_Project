// Test script to debug booking API
// Run in browser console: node test-booking-api.js

const API_URL = 'http://localhost:5031/api/bookings';

const testBooking = {
  roomId: 4,
  startUtc: new Date('2025-12-05T00:00:00Z').toISOString(),
  endUtc: new Date('2025-12-06T00:00:00Z').toISOString(),
  title: 'michael.eaton212@gmail.com'
};

console.log('Testing booking API...');
console.log('API URL:', API_URL);
console.log('Test Data:', JSON.stringify(testBooking, null, 2));

fetch(API_URL, {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify(testBooking)
})
  .then(response => {
    console.log('Response Status:', response.status);
    console.log('Response Headers:', [...response.headers.entries()]);
    return response.json().then(data => ({ status: response.status, data }));
  })
  .then(({ status, data }) => {
    console.log('Success!');
    console.log('Status:', status);
    console.log('Response Data:', JSON.stringify(data, null, 2));
  })
  .catch(error => {
    console.error('Error:', error);
  });
