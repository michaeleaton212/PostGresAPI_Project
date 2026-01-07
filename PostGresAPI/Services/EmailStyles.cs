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
        font-family: -apple-system, BlinkMacSystemFont, 'Inter', 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
        /* Grundsätzliche Schriftfarbe auf Schwarz gesetzt */
        color: #000000 !important;
    }
    
    body {
        font-family: -apple-system, BlinkMacSystemFont, 'Inter', 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
        background-color: #000000;
        padding: 20px;
        line-height: 1.6;
        -webkit-font-smoothing: antialiased;
    }
    
    .email-container {
        max-width: 600px;
        margin: 0 auto;
        /* Hintergrund hell angepasst, damit schwarze Schrift sichtbar ist */
        background-color: #F2F2F7; 
        border: 1px solid rgba(0, 0, 0, 0.1);
        border-radius: 40px;
        overflow: hidden;
        box-shadow: 0 25px 50px rgba(0,0,0,0.5);
    }
    
    .header {
        background: linear-gradient(180deg, rgba(10, 132, 255, 0.1) 0%, rgba(242, 242, 247, 0) 100%);
        padding: 50px 30px;
        text-align: center;
    }
    
    .header-icon {
        width: 80px;
        height: 80px;
        line-height: 80px;
        /* Liquid Glass Effekt */
        background: rgba(255, 255, 255, 0.5);
        backdrop-filter: blur(15px);
        -webkit-backdrop-filter: blur(15px);
        border: 1px solid rgba(0, 0, 0, 0.1);
        box-shadow: inset 0 0 15px rgba(255, 255, 255, 0.8), 0 10px 20px rgba(0, 0, 0, 0.05);
        
        border-radius: 26px;
        font-size: 38px;
        margin: 0 auto 25px;
        font-weight: 700;
        display: block;
    }
    
    .header h1 {
        font-size: 30px;
        font-weight: 700;
        letter-spacing: -0.5px;
    }
    
    .content {
        padding: 0 35px 45px;
    }
    
    .greeting {
        font-size: 24px;
        font-weight: 700;
        margin-bottom: 12px;
    }
    
    .intro {
        font-size: 17px;
        margin-bottom: 35px;
        opacity: 0.9;
    }
    
    .booking-card {
        background: rgba(0, 0, 0, 0.04);
        border: 1px solid rgba(0, 0, 0, 0.08);
        border-radius: 28px;
        padding: 25px;
        margin-bottom: 30px;
        text-align: center;
    }

    .booking-number-label {
        font-size: 13px;
        font-weight: 800;
        text-transform: uppercase;
        letter-spacing: 2px;
        margin-bottom: 8px;
        display: block;
    }
    
    .booking-number {
        font-size: 36px;
        font-weight: 800;
        letter-spacing: 3px;
    }
    
    .details-table {
        width: 100%;
        border-collapse: separate;
        border-spacing: 0 12px;
        margin-bottom: 35px;
    }
    
    .details-table td {
        padding: 16px 20px;
        background: rgba(0, 0, 0, 0.02);
        font-size: 16px;
    }
    
    .details-table td:first-child {
        border-radius: 18px 0 0 18px;
        width: 45%;
        font-weight: 500;
    }
    
    .details-table td:last-child {
        border-radius: 0 18px 18px 0;
        font-weight: 600;
        text-align: right;
    }
    
    .price-row td {
        background: rgba(10, 132, 255, 0.1) !important;
        border: 1px solid rgba(10, 132, 255, 0.2);
    }
    
    .price-row td:last-child {
        font-size: 24px;
        font-weight: 800;
    }
    
    .info-box {
        background: rgba(0, 0, 0, 0.03);
        border-radius: 28px;
        padding: 25px;
        border: 1px solid rgba(0, 0, 0, 0.05);
    }
    
    .info-box h3 {
        font-size: 19px;
        font-weight: 700;
        margin-bottom: 18px;
    }
    
    .info-box ul {
        list-style: none;
    }
    
    .info-box li {
        font-size: 15px;
        margin-bottom: 14px;
        padding-left: 30px;
        position: relative;
    }

    .info-box li::before {
        content: '✓';
        position: absolute;
        left: 0;
        font-weight: 900;
    }
    
    .footer {
        padding: 45px 30px;
        text-align: center;
        font-size: 14px;
        opacity: 0.7;
    }
    
    .footer-copyright {
        margin-top: 25px;
        padding-top: 25px;
        border-top: 1px solid rgba(0, 0, 0, 0.05);
    }
</style>";
        }
    }
}