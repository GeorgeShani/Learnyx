namespace learnyx.SMTP.Templates;

public static class EmailTemplates
{
    public static string GetWelcomeEmailTemplate(string firstName)
    {
      return $"""
      <!DOCTYPE html>
      <html lang="en">
        <head>
          <meta charset="UTF-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1.0" />
          <title>Welcome to Learnyx - Your Learning Journey Begins!</title>
        </head>
        <body
          style="
            margin: 0;
            padding: 0;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto,
              sans-serif;
            background-color: #f8fafc;
            line-height: 1.6;
          "
        >
          <!-- Email Container -->
          <table role="presentation" style="width: 100%; border-collapse: collapse">
            <tr>
              <td style="padding: 40px 20px">
                <!-- Main Email Content -->
                <table
                  role="presentation"
                  style="
                    max-width: 600px;
                    margin: 0 auto;
                    background-color: #ffffff;
                    border-radius: 16px;
                    box-shadow: 0 10px 25px rgba(0, 0, 0, 0.1);
                    overflow: hidden;
                  "
                >
                  <!-- Header with Gradient -->
                  <tr>
                    <td
                      style="
                        background: linear-gradient(
                          135deg,
                          hsl(178, 60%, 70%) 0%,
                          hsl(220, 85%, 70%) 100%
                        );
                        padding: 40px 30px;
                        text-align: center;
                      "
                    >
                      <!-- Logo -->
                      <div style="margin-bottom: 20px">
                        <div
                          style="
                            display: inline-flex;
                            align-items: center;
                            justify-content: center;
                            width: 60px;
                            height: 60px;
                            border-radius: 16px;
                            backdrop-filter: blur(10px);
                          "
                        >
                          <img
                            src="https://learnyx-storage-bucket.s3.eu-north-1.amazonaws.com/general/learnyx.svg"
                            alt="Learnyx logo"
                          />
                        </div>
                      </div>
                      <h1
                        style="
                          margin: 0;
                          color: white;
                          font-size: 32px;
                          font-weight: 700;
                          text-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                        "
                      >
                        Welcome to Learnyx!
                      </h1>
                      <p
                        style="
                          margin: 10px 0 0 0;
                          color: rgba(255, 255, 255, 0.9);
                          font-size: 18px;
                          font-weight: 400;
                        "
                      >
                        Your learning journey starts here
                      </p>
                    </td>
                  </tr>

                  <!-- Main Content -->
                  <tr>
                    <td style="padding: 40px 30px">
                      <h2
                        style="
                          margin: 0 0 20px 0;
                          color: hsl(220, 15%, 20%);
                          font-size: 24px;
                          font-weight: 600;
                        "
                      >
                        Hi there, {firstName}! 👋
                      </h2>

                      <p
                        style="
                          margin: 0 0 20px 0;
                          color: hsl(220, 15%, 40%);
                          font-size: 16px;
                        "
                      >
                        We're thrilled to have you join the Learnyx community! You've
                        just unlocked access to thousands of courses, expert
                        instructors, and a world of knowledge at your fingertips.
                      </p>

                      <!-- CTA Button -->
                      <div style="text-align: center; margin: 30px 0">
                        <a
                          href="#"
                          style="
                            display: inline-block;
                            background: linear-gradient(
                              135deg,
                              hsl(178, 60%, 70%) 0%,
                              hsl(220, 85%, 70%) 100%
                            );
                            color: white;
                            text-decoration: none;
                            padding: 16px 32px;
                            border-radius: 12px;
                            font-weight: 600;
                            font-size: 16px;
                            box-shadow: 0 4px 12px rgba(111, 220, 217, 0.4);
                            transition: all 0.3s ease;
                          "
                        >
                          Start Learning Now
                        </a>
                      </div>

                      <!-- Features Grid -->
                      <div style="margin: 40px 0">
                        <h3
                          style="
                            margin: 0 0 25px 0;
                            color: hsl(220, 15%, 20%);
                            font-size: 20px;
                            font-weight: 600;
                            text-align: center;
                          "
                        >
                          What you can do with Learnyx:
                        </h3>

                        <!-- Feature Items -->
                        <table
                          role="presentation"
                          style="width: 100%; border-collapse: collapse"
                        >
                          <tr>
                            <td
                              style="width: 50%; padding: 15px; vertical-align: top"
                            >
                              <div style="display: flex; align-items: flex-start">
                                <div
                                  style="
                                    flex-shrink: 0;
                                    width: 40px;
                                    height: 40px;
                                    background: linear-gradient(
                                      135deg,
                                      hsl(178, 60%, 70%) 0%,
                                      hsl(220, 85%, 70%) 100%
                                    );
                                    border-radius: 10px;
                                    display: flex;
                                    align-items: center;
                                    justify-content: center;
                                    margin-right: 15px;
                                  "
                                >
                                  <svg
                                    width="20"
                                    height="20"
                                    viewBox="0 0 24 24"
                                    fill="none"
                                    stroke="white"
                                    stroke-width="2"
                                    stroke-linecap="round"
                                    stroke-linejoin="round"
                                  >
                                    <circle cx="12" cy="12" r="10" />
                                    <polygon points="10,8 16,12 10,16 10,8" />
                                  </svg>
                                </div>
                                <div>
                                  <h4
                                    style="
                                      margin: 0 0 5px 0;
                                      color: hsl(220, 15%, 20%);
                                      font-size: 16px;
                                      font-weight: 600;
                                    "
                                  >
                                    Watch Courses
                                  </h4>
                                  <p
                                    style="
                                      margin: 0;
                                      color: hsl(220, 15%, 50%);
                                      font-size: 14px;
                                    "
                                  >
                                    Access thousands of video lessons
                                  </p>
                                </div>
                              </div>
                            </td>
                            <td
                              style="width: 50%; padding: 15px; vertical-align: top"
                            >
                              <div style="display: flex; align-items: flex-start">
                                <div
                                  style="
                                    flex-shrink: 0;
                                    width: 40px;
                                    height: 40px;
                                    background: linear-gradient(
                                      135deg,
                                      hsl(178, 60%, 70%) 0%,
                                      hsl(220, 85%, 70%) 100%
                                    );
                                    border-radius: 10px;
                                    display: flex;
                                    align-items: center;
                                    justify-content: center;
                                    margin-right: 15px;
                                  "
                                >
                                  <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    width="24"
                                    height="24"
                                    viewBox="0 0 24 24"
                                    fill="none"
                                    stroke="#ffffff"
                                    stroke-width="2"
                                    stroke-linecap="round"
                                    stroke-linejoin="round"
                                  >
                                    <path
                                      d="M21.42 10.922a1 1 0 0 0-.019-1.838L12.83 5.18a2 2 0 0 0-1.66 0L2.6 9.08a1 1 0 0 0 0 1.832l8.57 3.908a2 2 0 0 0 1.66 0z"
                                    />
                                    <path d="M22 10v6" />
                                    <path d="M6 12.5V16a6 3 0 0 0 12 0v-3.5" />
                                  </svg>
                                </div>
                                <div>
                                  <h4
                                    style="
                                      margin: 0 0 5px 0;
                                      color: hsl(220, 15%, 20%);
                                      font-size: 16px;
                                      font-weight: 600;
                                    "
                                  >
                                    Track Progress
                                  </h4>
                                  <p
                                    style="
                                      margin: 0;
                                      color: hsl(220, 15%, 50%);
                                      font-size: 14px;
                                    "
                                  >
                                    Monitor your learning journey
                                  </p>
                                </div>
                              </div>
                            </td>
                          </tr>
                          <tr>
                            <td
                              style="width: 50%; padding: 15px; vertical-align: top"
                            >
                              <div style="display: flex; align-items: flex-start">
                                <div
                                  style="
                                    flex-shrink: 0;
                                    width: 40px;
                                    height: 40px;
                                    background: linear-gradient(
                                      135deg,
                                      hsl(178, 60%, 70%) 0%,
                                      hsl(220, 85%, 70%) 100%
                                    );
                                    border-radius: 10px;
                                    display: flex;
                                    align-items: center;
                                    justify-content: center;
                                    margin-right: 15px;
                                  "
                                >
                                  <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    width="24"
                                    height="24"
                                    viewBox="0 0 24 24"
                                    fill="none"
                                    stroke="#ffffff"
                                    stroke-width="2"
                                    stroke-linecap="round"
                                    stroke-linejoin="round"
                                  >
                                    <path
                                      d="M20 13c0 5-3.5 7.5-7.66 8.95a1 1 0 0 1-.67-.01C7.5 20.5 4 18 4 13V6a1 1 0 0 1 1-1c2 0 4.5-1.2 6.24-2.72a1.17 1.17 0 0 1 1.52 0C14.51 3.81 17 5 19 5a1 1 0 0 1 1 1z"
                                    />
                                    <path d="m9 12 2 2 4-4" />
                                  </svg>
                                </div>
                                <div>
                                  <h4
                                    style="
                                      margin: 0 0 5px 0;
                                      color: hsl(220, 15%, 20%);
                                      font-size: 16px;
                                      font-weight: 600;
                                    "
                                  >
                                    Earn Certificates
                                  </h4>
                                  <p
                                    style="
                                      margin: 0;
                                      color: hsl(220, 15%, 50%);
                                      font-size: 14px;
                                    "
                                  >
                                    Get recognized for your achievements
                                  </p>
                                </div>
                              </div>
                            </td>
                            <td
                              style="width: 50%; padding: 15px; vertical-align: top"
                            >
                              <div style="display: flex; align-items: flex-start">
                                <div
                                  style="
                                    flex-shrink: 0;
                                    width: 40px;
                                    height: 40px;
                                    background: linear-gradient(
                                      135deg,
                                      hsl(178, 60%, 70%) 0%,
                                      hsl(220, 85%, 70%) 100%
                                    );
                                    border-radius: 10px;
                                    display: flex;
                                    align-items: center;
                                    justify-content: center;
                                    margin-right: 15px;
                                  "
                                >
                                  <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    width="24"
                                    height="24"
                                    viewBox="0 0 24 24"
                                    fill="none"
                                    stroke="#ffffff"
                                    stroke-width="2"
                                    stroke-linecap="round"
                                    stroke-linejoin="round"
                                  >
                                    <path
                                      d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"
                                    />
                                    <path d="M16 3.128a4 4 0 0 1 0 7.744" />
                                    <path d="M22 21v-2a4 4 0 0 0-3-3.87" />
                                    <circle cx="9" cy="7" r="4" />
                                  </svg>
                                </div>
                                <div>
                                  <h4
                                    style="
                                      margin: 0 0 5px 0;
                                      color: hsl(220, 15%, 20%);
                                      font-size: 16px;
                                      font-weight: 600;
                                    "
                                  >
                                    Join Community
                                  </h4>
                                  <p
                                    style="
                                      margin: 0;
                                      color: hsl(220, 15%, 50%);
                                      font-size: 14px;
                                    "
                                  >
                                    Connect with fellow learners
                                  </p>
                                </div>
                              </div>
                            </td>
                          </tr>
                        </table>
                      </div>

                      <!-- Quick Start Tips -->
                      <div
                        style="
                          background: linear-gradient(
                            135deg,
                            hsl(178, 60%, 95%) 0%,
                            hsl(220, 85%, 95%) 100%
                          );
                          border-radius: 12px;
                          padding: 25px;
                          margin: 30px 0;
                        "
                      >
                        <h3
                          style="
                            margin: 0 0 15px 0;
                            color: hsl(220, 15%, 20%);
                            font-size: 18px;
                            font-weight: 600;
                          "
                        >
                          Quick Start Tips:
                        </h3>
                        <ul
                          style="
                            margin: 0;
                            padding-left: 20px;
                            color: hsl(220, 15%, 40%);
                          "
                        >
                          <li style="margin-bottom: 8px">
                            Complete your profile to get personalized course
                            recommendations
                          </li>
                          <li style="margin-bottom: 8px">
                            Browse our featured courses to find something that
                            interests you
                          </li>
                          <li style="margin-bottom: 8px">
                            Set learning goals and track your progress in your
                            dashboard
                          </li>
                          <li style="margin-bottom: 0">
                            Join course discussions to connect with other learners
                          </li>
                        </ul>
                      </div>

                      <!-- Support Section -->
                      <div style="text-align: center; margin: 30px 0">
                        <p
                          style="
                            margin: 0 0 15px 0;
                            color: hsl(220, 15%, 40%);
                            font-size: 14px;
                          "
                        >
                          Need help getting started? Our support team is here for you.
                        </p>
                        <a
                          href="http://localhost:4200/help"
                          style="
                            color: hsl(178, 60%, 50%);
                            text-decoration: none;
                            font-weight: 600;
                            font-size: 14px;
                          "
                        >
                          Visit Help Center →
                        </a>
                      </div>
                    </td>
                  </tr>

                  <!-- Footer -->
                  <tr>
                    <td
                      style="
                        background-color: hsl(220, 15%, 98%);
                        padding: 30px;
                        text-align: center;
                        border-top: 1px solid hsl(220, 15%, 90%);
                      "
                    >
                      <p
                        style="
                          margin: 0 0 15px 0;
                          color: hsl(220, 15%, 60%);
                          font-size: 14px;
                        "
                      >
                        Happy learning!<br />
                        The Learnyx Team
                      </p>

                      <p
                        style="
                          margin: 15px 0 0 0;
                          color: hsl(220, 15%, 70%);
                          font-size: 12px;
                        "
                      >
                        © 2025 Learnyx. All rights reserved.<br />
                        <a
                          href="#"
                          style="color: hsl(220, 15%, 70%); text-decoration: none"
                        >
                          Unsubscribe
                        </a>
                        |
                        <a
                          href="http://localhost:4200/privacy"
                          style="color: hsl(220, 15%, 70%); text-decoration: none"  
                        >
                          Privacy Policy
                        </a>
                      </p>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>
        </body>
      </html>
      """;
    }
  
    public static string GetVerificationEmailTemplate(string verificationCode)
    {
      return $"""
      <!DOCTYPE html>
      <html lang="en">
        <head>
          <meta charset="UTF-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1.0" />
          <title>Reset Your Password - Learnyx</title>
        </head>
        <body
          style="
            margin: 0;
            padding: 0;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
            background-color: #f8fafc;
            line-height: 1.6;
          "
        >
          <table role="presentation" style="width: 100%; border-collapse: collapse">
            <tr>
              <td style="padding: 40px 20px">
                <table
                  role="presentation"
                  style="
                    max-width: 600px;
                    margin: 0 auto;
                    background-color: #ffffff;
                    border-radius: 16px;
                    box-shadow: 0 10px 25px rgba(0, 0, 0, 0.1);
                    overflow: hidden;
                  "
                >
                  <!-- Header -->
                  <tr>
                    <td
                      style="
                        background: linear-gradient(135deg, #6fdcd9 0%, #749bfb 100%);
                        padding: 40px 40px 30px;
                        text-align: center;
                      "
                    >
                      <div
                        style="
                          display: inline-flex;
                          align-items: center;
                          gap: 12px;
                          margin-bottom: 20px;
                        "
                      >
                        <div
                          style="
                            width: 40px;
                            height: 40px;
                            border-radius: 12px;
                            display: flex;
                            align-items: center;
                            justify-content: center;
                          "
                        >
                          <img src="https://learnyx-storage-bucket.s3.eu-north-1.amazonaws.com/general/learnyx.svg" alt="Learnyx logo">
                        </div>
                        <h1
                          style="
                            margin: 0;
                            color: white;
                            font-size: 28px;
                            font-weight: 700;
                            letter-spacing: -0.5px;
                          "
                        >
                          Learnyx
                        </h1>
                      </div>
                      <h2
                        style="
                          margin: 0;
                          color: white;
                          font-size: 24px;
                          font-weight: 600;
                          opacity: 0.95;
                        "
                      >
                        Reset Your Password
                      </h2>
                    </td>
                  </tr>

                  <!-- Content -->
                  <tr>
                    <td style="padding: 40px">
                      <div style="text-align: center; margin-bottom: 32px">
                        <p
                          style="
                            margin: 0 0 24px;
                            color: #334155;
                            font-size: 16px;
                            line-height: 1.6;
                          "
                        >
                          We received a request to reset your password for your
                          Learnyx account. Enter the verification code below to
                          proceed with resetting your password:
                        </p>
                        <div
                          style="
                            background: linear-gradient(
                              135deg,
                              #f1f5f9 0%,
                              #e2e8f0 100%
                            );
                            border: 2px solid #e2e8f0;
                            border-radius: 16px;
                            padding: 32px;
                            margin: 32px 0;
                            display: inline-block;
                          "
                        >
                          <p
                            style="
                              margin: 0 0 8px;
                              color: #64748b;
                              font-size: 14px;
                              font-weight: 500;
                              text-transform: uppercase;
                              letter-spacing: 0.5px;
                            "
                          >
                            Password Reset Code
                          </p>
                          <div
                            style="
                              font-family: 'Courier New', monospace;
                              font-size: 36px;
                              font-weight: 700;
                              color: #1e293b;
                              letter-spacing: 8px;
                              margin: 0;
                            "
                          >
                            {verificationCode}
                          </div>
                        </div>

                        <div
                          style="
                            background: #fef3c7;
                            border: 1px solid #fbbf24;
                            border-radius: 12px;
                            padding: 16px;
                            margin: 24px 0;
                            text-align: left;
                          "
                        >
                          <div
                            style="display: flex; align-items: flex-start; gap: 12px"
                          >
                            <svg
                              width="20"
                              height="20"
                              viewBox="0 0 24 24"
                              fill="none"
                              stroke="#d97706"
                              stroke-width="2"
                              stroke-linecap="round"
                              stroke-linejoin="round"
                              style="margin-top: 2px; flex-shrink: 0"
                            >
                              <path
                                d="m21.73 18-8-14a2 2 0 0 0-3.48 0l-8 14A2 2 0 0 0 4 21h16a2 2 0 0 0 1.73-3Z"
                              />
                              <path d="M12 9v4" />
                              <path d="m12 17 .01 0" />
                            </svg>
                            <div>
                              <p
                                style="
                                  margin: 0 0 4px;
                                  color: #92400e;
                                  font-size: 14px;
                                  font-weight: 600;
                                "
                              >
                                Important Security Notice
                              </p>
                              <p
                                style="
                                  margin: 0;
                                  color: #92400e;
                                  font-size: 13px;
                                  line-height: 1.4;
                                "
                              >
                                This code expires in 10 minutes and can only be used
                                once. Never share this code with anyone.
                              </p>
                            </div>
                          </div>
                        </div>
                      </div>

                      <!-- Instructions -->
                      <div
                        style="
                          background: #f8fafc;
                          border-radius: 12px;
                          padding: 24px;
                          margin: 24px 0;
                        "
                      >
                        <h3
                          style="
                            margin: 0 0 16px;
                            color: #1e293b;
                            font-size: 16px;
                            font-weight: 600;
                          "
                        >
                          How to reset your password:
                        </h3>
                        <ol
                          style="
                            margin: 0;
                            padding-left: 20px;
                            color: #475569;
                            font-size: 14px;
                            line-height: 1.6;
                          "
                        >
                          <li style="margin-bottom: 8px">
                            Return to the Learnyx password reset page
                          </li>
                          <li style="margin-bottom: 8px">
                            Enter the 6-digit verification code above
                          </li>
                          <li style="margin-bottom: 8px">
                            Create a new secure password
                          </li>
                          <li style="margin-bottom: 0">
                            Log in with your new password
                          </li>
                        </ol>
                      </div>

                      <!-- Didn't request this? -->
                      <div
                        style="
                          text-align: center;
                          margin-top: 32px;
                          padding-top: 24px;
                          border-top: 1px solid #e2e8f0;
                        "
                      >
                        <div
                          style="
                            background: #fef2f2;
                            border: 1px solid #fecaca;
                            border-radius: 8px;
                            padding: 16px;
                            margin-bottom: 16px;
                          "
                        >
                          <p
                            style="
                              margin: 0;
                              color: #b91c1c;
                              font-size: 14px;
                              font-weight: 500;
                            "
                          >
                            Didn't request a password reset? Your account may be at
                            risk.
                            <a
                              href="#"
                              style="
                                color: #dc2626;
                                text-decoration: none;
                                font-weight: 600;
                              "
                              >Secure your account immediately</a
                            >
                          </p>
                        </div>
                        <p style="margin: 0; color: #64748b; font-size: 14px">
                          Need help?
                          <a
                            href="http://localhost:4200/contact"
                            style="
                              color: #6fdcd9;
                              text-decoration: none;
                              font-weight: 500;
                            "
                            >Contact our support team</a
                          >
                        </p>
                      </div>
                    </td>
                  </tr>

                  <!-- Footer -->
                  <tr>
                    <td
                      style="
                        background: #f8fafc;
                        padding: 32px 40px;
                        text-align: center;
                        border-top: 1px solid #e2e8f0;
                      "
                    >
                      <p style="margin: 0 0 16px; color: #64748b; font-size: 14px">
                        This password reset email was sent by Learnyx. If you have any
                        questions, please contact our support team.
                      </p>
                      <div style="margin: 16px 0">
                        <a
                          href="http://localhost:4200/help"
                          style="
                            color: #6fdcd9;
                            text-decoration: none;
                            font-size: 13px;
                            margin: 0 12px;
                          "
                          >Help Center</a
                        >
                        <span style="color: #cbd5e1">•</span>
                        <a
                          href="http://localhost:4200/privacy"
                          style="
                            color: #6fdcd9;
                            text-decoration: none;
                            font-size: 13px;
                            margin: 0 12px;
                          "
                          >Privacy Policy</a
                        >
                        <span style="color: #cbd5e1">•</span>
                        <a
                          href="http://localhost:4200/terms"
                          style="
                            color: #6fdcd9;
                            text-decoration: none;
                            font-size: 13px;
                            margin: 0 12px;
                          "
                          >Terms of Service</a
                        >
                      </div>
                      <p style="margin: 16px 0 0; color: #94a3b8; font-size: 12px">
                        © 2025 Learnyx. All rights reserved.
                      </p>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>
        </body>
      </html>
      """;
    }
}