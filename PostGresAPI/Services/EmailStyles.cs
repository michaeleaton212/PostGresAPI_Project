namespace PostGresAPI.Services
{
    public static class EmailStyles
    {
        public static string GetBookingConfirmationStyles()
        {
            return @"
<style>
    * {
        margin: 0;
        padding: 0;
        box-sizing: border-box;
    }
    
    body {
        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        background-color: #f0f2f5;
        padding: 20px;
        line-height: 1.6;
    }
    
    .email-container {
        max-width: 600px;
        margin: 0 auto;
        background-color: #ffffff;
        border-radius: 12px;
        overflow: hidden;
        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    }
    
    .header {
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: white;
        padding: 40px 30px;
        text-align: center;
    }
    
    .header h1 {
        font-size: 28px;
        font-weight: 600;
        margin: 0;
    }
    
    .header-icon {
        font-size: 48px;
        margin-bottom: 10px;
    }
    
    .content {
        padding: 40px 30px;
    }
    
    .greeting {
        font-size: 18px;
        color: #333;
        margin-bottom: 20px;
    }
    
    .intro {
        color: #666;
        margin-bottom: 30px;
    }
    
    .booking-number-section {
        text-align: center;
        margin: 30px 0;
    }
    
    .booking-number {
        display: inline-block;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: white;
        padding: 15px 30px;
        border-radius: 8px;
        font-size: 24px;
        font-weight: bold;
        letter-spacing: 2px;
    }
    
    .booking-number-label {
        display: block;
        font-size: 14px;
        color: #666;
        margin-bottom: 10px;
        font-weight: 600;
        text-transform: uppercase;
    }
    
    .details-table {
        width: 100%;
        border-collapse: collapse;
        margin: 30px 0;
        background-color: #f8f9fa;
        border-radius: 8px;
        overflow: hidden;
    }
    
    .details-table tr {
        border-bottom: 1px solid #e9ecef;
    }
    
    .details-table tr:last-child {
        border-bottom: none;
    }
    
    .details-table td {
        padding: 16px 20px;
    }
    
    .details-table td:first-child {
        font-weight: 600;
        color: #667eea;
        width: 45%;
        background-color: #ffffff;
    }
    
    .details-table td:last-child {
        color: #333;
        background-color: #f8f9fa;
    }
    
    .price-row {
        background: linear-gradient(135deg, #e8f0ff 0%, #f0e8ff 100%);
    }
    
    .price-row td {
        font-size: 20px;
        font-weight: bold;
        color: #667eea !important;
        padding: 20px !important;
    }
    
    .info-box {
        background-color: #e8f4fd;
        border-left: 4px solid #667eea;
        padding: 20px;
        border-radius: 8px;
        margin: 30px 0;
    }
    
    .info-box h3 {
        color: #667eea;
        font-size: 16px;
        margin-bottom: 12px;
        font-weight: 600;
    }
    
    .info-box ul {
        list-style: none;
        padding: 0;
        margin: 0;
    }
    
    .info-box li {
        color: #555;
        margin-bottom: 8px;
        padding-left: 20px;
        position: relative;
    }
    
    .info-box li:before {
        content: '?';
        position: absolute;
        left: 0;
        color: #667eea;
        font-weight: bold;
    }
    
    .footer {
        background-color: #f8f9fa;
        padding: 30px;
        text-align: center;
        color: #666;
        font-size: 13px;
        border-top: 1px solid #e9ecef;
    }
    
    .footer p {
        margin: 8px 0;
    }
    
    .footer-copyright {
        margin-top: 20px;
        padding-top: 20px;
        border-top: 1px solid #dee2e6;
        font-size: 12px;
        color: #999;
    }
    
    .icon {
        display: inline-block;
        margin-right: 8px;
    }
</style>";
        }
    }
}
