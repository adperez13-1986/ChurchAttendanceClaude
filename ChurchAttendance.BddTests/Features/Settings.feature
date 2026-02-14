Feature: Settings
  As a church administrator
  I want to configure email settings
  So that I can send attendance reports via email

  Scenario: Save SMTP settings
    Given the application is running
    When I save SMTP settings with host "smtp.gmail.com" port 587 and email "test@example.com"
    Then the SMTP settings should be saved
    And the saved host should be "smtp.gmail.com"

  Scenario: Email report requires SMTP configuration
    Given the application is running
    And no SMTP settings are configured
    When I try to email a report
    Then I should receive an error about missing SMTP configuration
