# Safe Future Barcode for Good

![Safe Future Foundation Logo](SafeFutureInventorySystem/wwwroot/images/safe-future-logo.png)

A professional inventory and barcode web application built for Safe Future Foundation, Inc. to support diaper-bank and community resource operations with reliable item tracking, QR workflows, and reporting.

Official organization website: https://www.asafefuture.org/

## Audience Guide (One README for Two Audiences)

This document is intentionally written for both non-technical stakeholders and technical maintainers.

- If you are a client stakeholder, operations lead, or reviewer:
  - Start with Executive Summary, Client Context, Original Use Case and Business Need, Proposal Feature Alignment, Impact Statement.
  - Then review Quick Start and Production Readiness Checklist for rollout confidence.
- If you are a developer or technical maintainer:
  - Start with Quick Start, Technology Stack, Architecture Overview, Data Model Summary, Configuration, Operational Workflows.
  - Then review Deployment and Production Readiness Notes, Known Gaps, and Recommended Next Phase.

Reading time guidance:

- Business overview path: about 8 to 12 minutes
- Technical handoff path: about 15 to 25 minutes

## Document Control

- Project Name: Safe Future Barcode for Good
- Client: Safe Future Foundation, Inc.
- Repository: SafeFutureInventorySystem
- Current Version: v1.0.0-client-handoff
- Last Updated: 2026-04-05
- Primary Environment: ASP.NET Core MVC on .NET 9 with SQLite

## Quick Start (Client and Reviewer)

Use this section if you only need to run and verify the system quickly.

1. Build and run:

```bash
dotnet restore
dotnet build
dotnet run --project SafeFutureInventorySystem
```

2. Open in browser:

- https://localhost:7216
- http://localhost:5174

3. Sign in with seeded bootstrap account:

- Email: admin@safefuture.com
- Password: admin1234

4. Immediate first action:

- Change the admin password after first successful login.

5. Verify core client workflows:

- Create or merge an inventory item.
- Open item details and confirm QR generation.
- Scan QR on mobile and verify the item details page opens.
- Print selected QR labels from the inventory list.
- Export inventory as CSV, PDF, and Excel.

## Executive Summary [Business]

This project was delivered in response to the original Safe Future Foundation proposal for a custom barcode-enabled inventory platform.

The application helps the organization:

- Track incoming and outgoing donated inventory with auditable quantity updates.
- Reduce manual errors in stock handling and recordkeeping.
- Improve visibility into inventory status for operations and reporting.
- Support mission delivery for families in need by making distribution logistics more reliable.

## Client Context [Business]

Safe Future Foundation, Inc. is a Jacksonville, Florida nonprofit focused on supporting children and families through programs such as diaper distribution, food support, education, and community outreach. The inventory system in this repository is designed to support those operational goals.

Client reference information from the original proposal:

- Organization: Safe Future Foundation, Inc.
- Website: https://www.asafefuture.org/
- Contact: Chris Tobey
- Email: Info@asafefuture.org
- Phone: 904-803-0034

## Original Use Case and Business Need [Business]

The foundation requested a system to:

- Prevent inventory loss and reselling by improving item traceability.
- Keep accurate counts of items received and distributed.
- Generate item labels that combine machine-readable QR codes with human-readable information.
- Enable scan-to-open item pages from mobile devices.
- Produce downloadable inventory reports for internal and external stakeholders.

The delivered application provides the core technical foundation for these needs and includes a roadmap for remaining enhancements.

## Proposal Feature Alignment [Business + Technical]

| Requested Capability | Status | Notes |
|---|---|---|
| Mobile and desktop accessibility | Implemented | Responsive ASP.NET MVC web app available through browser on desktop and phone. |
| Generate QR code labels with human-readable context | Implemented | QR generation is implemented, resolves to item details pages, and supports batch printable QR label sheets for selected items. |
| Add product info with QR linkage | Implemented | Item creation and persistence are in place with associated QR generation. |
| Scan QR to open product page and act | Implemented | QR points to item details route where users can review and manage item data. |
| Download all inventory data | Implemented (partial) | CSV, PDF, and Excel exports are available. Full DB dump endpoint not yet implemented. |
| Search and view product information | Implemented | Search, filters, sorting, and details pages are available. |
| Simple login protection | Implemented | ASP.NET Identity with role-based access and login flow is active. |
| Upload inventory information | Planned | Upload/import workflow is a recommended next enhancement. |
| Low-stock alerts and monitoring | Planned | Expiration monitoring exists; low-stock alert thresholds can be added next. |
| Reports and summaries | Implemented (core) | PDF and CSV reporting available; richer dashboards can be added. |

## Current Solution Highlights [Business + Technical]

- Secure account system with Admin and Volunteer roles.
- Inventory management with duplicate-merge logic for cleaner stock records.
- Donation and adjustment logs for operational traceability.
- Expiration-state awareness for proactive stock management.
- QR code generation for direct scan-to-item workflows.
- Batch printable QR labels for selected inventory items.
- PDF, CSV, and Excel export for reporting.
- Centralized error handling and logging support.

## Branding and Logo Assets [Business + Technical]

The repository includes multiple organization logo files for use in UI polish, printed materials, and deployment branding:

- SafeFutureInventorySystem/wwwroot/images/safe-future-logo.png
- SafeFutureInventorySystem/wwwroot/images/safefuture-logo-original.png
- SafeFutureInventorySystem/wwwroot/images/safefuturelogo.webp

If preferred, this README image can be switched to any of the alternatives above.

## Technology Stack [Technical]

- ASP.NET Core MVC (.NET 9)
- Entity Framework Core 9
- ASP.NET Core Identity
- SQLite
- ZXing.Net and SkiaSharp (QR generation)
- iTextSharp (PDF export)
- EPPlus (Excel export)

## Architecture Overview [Technical]

### Application Layer

- MVC controllers coordinate auth, inventory actions, exports, and QR endpoints.
- Razor views provide browser-based UX for staff and volunteers.
- Static resources are served from wwwroot.

### Data Layer

- ApplicationDbContext stores inventory entities and history logs.
- AuthDbContext stores users, roles, and authentication state.

### Startup Behavior

At application startup:

- Inventory and auth databases are created if missing.
- Roles are seeded: Admin and Volunteer.
- A default admin account is seeded if absent.

## Data Model Summary [Technical]

### InventoryItem

- Name, description, quantity, barcode, category
- ExpirationDate, DateAdded, LastUpdated
- Computed ExpirationStatus (Expired, Expiring Soon, Expiring This Month, Good, No Expiration)
- One-to-many relationship with donation and adjustment logs

### DonationLog

- Tracks donation events by item and quantity with optional donor context

### InventoryAdjustmentLog

- Tracks manual quantity changes, reason, user context, and timestamps

### ApplicationUser

- ASP.NET Identity user extended with first and last name

## Getting Started [Technical]

### Prerequisites

- .NET SDK 9.0 or later
- Windows, macOS, or Linux

### Run Locally

```bash
dotnet restore
dotnet build
dotnet run --project SafeFutureInventorySystem
```

Default development URLs:

- http://localhost:5174
- https://localhost:7216

## Configuration [Technical]

Main configuration file: SafeFutureInventorySystem/appsettings.json

Important settings:

- ConnectionStrings:DefaultConnection (inventory database)
- ConnectionStrings:AuthConnection (identity/auth database)
- QrCodeBaseUrl (public base URL for QR links)

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=SafeFutureInventory.db",
    "AuthConnection": "Data Source=SafeFutureAuth.db"
  },
  "QrCodeBaseUrl": ""
}
```

Production recommendation:

Set QrCodeBaseUrl to the public domain so mobile scans resolve outside local networks.

## Authentication and Access [Business + Technical]

- Login route: /Account/Login
- Seeded roles: Admin, Volunteer
- Cookie session: 8 hours with sliding expiration

Seeded bootstrap admin account:

- Email: admin@safefuture.com
- Password: admin1234

Important:

Change this password immediately in staging/production environments.

## Operational Workflows [Business + Technical]

### Inventory Search and Review

- Search by name, description, barcode
- Filter by expiration condition and date window
- Sort and paginate for high-volume inventories

### Item Intake and Merge Logic

When staff adds an item:

1. Match by barcode first.
2. If no barcode match, match by normalized name.
3. Merge quantity into existing item when matched.
4. Create a new item when no match is found.

### Adjustment and Audit Trail

- Quantity changes are logged with old/new values, reason, timestamp, and user context.

### Exports

- PDF inventory report endpoint
- CSV inventory export endpoint
- Excel inventory export endpoint

## QR and Scan Flow [Business + Technical]

- QR code values are generated for item details links.
- Scan opens the associated item details page.
- URL generation supports configured public base URL or request-host fallback.
- Staff can select multiple items from the inventory list and print a batch QR label sheet.
- Batch QR labels support multiple print layouts and label templates for denser or more detailed sheets.
- Batch QR labels support named stock presets and PDF export for print-ready sharing.

### Batch QR Label Workflow

1. Open the inventory list.
2. Select one or more items using the table checkboxes.
3. Click `Print Selected QR Labels`.
4. The app routes to the QR batch label page and passes the selected item IDs as query parameters.
5. Choose the preferred stock preset, label template, and print layout.
6. Print the sheet directly or export the batch as PDF.

## Deployment and Production Readiness Notes [Technical]

This project is production-capable with additional hardening recommended before client go-live:

- Move from EnsureCreated to migration-driven startup and release flow.
- Strengthen password requirements and add account lockout policy.
- Add explicit authorization attributes on all sensitive endpoints.
- Add environment-specific secrets management and HTTPS certificate strategy.
- Add CI checks for build, tests, and security scanning.

## Production Readiness Checklist [Business + Technical]

Status key: Complete, In Progress, Planned

- Authentication and role-based access: Complete
- Centralized error handling and logging: Complete
- QR-based item traceability: Complete
- Export for reporting (CSV/PDF/Excel): Complete
- Password policy hardening for production: Planned
- Migration-based deployment strategy: Planned
- Full endpoint authorization audit: In Progress
- Backup/restore runbook: Planned
- Monitoring and alerting strategy: Planned

## Support and Ownership [Business + Technical]

### Client Organization

- Safe Future Foundation, Inc.
- Contact: Chris Tobey
- Email: Info@asafefuture.org
- Phone: 904-803-0034

### Recommended Technical Ownership Model

- Product Owner (Client): Approves workflow changes and operational priorities.
- Technical Maintainer (Student team or designated successor): Implements updates and release fixes.
- Operations Contact (Client): Confirms day-to-day usability and issue priority.

### Change Request Process (Recommended)

1. Submit request with business impact, urgency, and expected behavior.
2. Triage request as bug, enhancement, or compliance requirement.
3. Estimate effort and deployment risk.
4. Approve and schedule into release window.
5. Validate in staging before production rollout.

## Operational Handoff Notes [Technical]

### Backups

- Inventory data file: SafeFutureInventory.db
- Auth data file: SafeFutureAuth.db
- Minimum recommendation: daily backup with weekly restore verification.

### Release and Rollback (Recommended)

1. Build and test in staging environment.
2. Back up both SQLite files prior to deployment.
3. Deploy new build.
4. Validate login, search, QR scan, and export paths.
5. If rollback is needed, redeploy previous build and restore latest validated database backup.

## Suggested Screenshot Set for Stakeholder Packet [Business]

To make this README client-ready for board members, funders, or operations staff, include the following screenshots:

- Login screen
- Inventory list with filters and search
- Item details page with quantity history
- QR generation and mobile scan result
- CSV and PDF export examples

Suggested location for these assets:

- docs/screenshots/login.png
- docs/screenshots/inventory-list.png
- docs/screenshots/item-details.png
- docs/screenshots/qr-scan-flow.png
- docs/screenshots/reports-export.png

## Known Gaps Against Full Proposal Scope [Business + Technical]

- Inventory upload/import is not implemented yet.
- Low-stock threshold alerting is not implemented yet.
- Rich operational dashboards and KPI summaries can be expanded further.

## Recommended Next Phase [Business + Technical]

1. Add CSV/Excel inventory import with validation and dry-run preview.
2. Add low-stock thresholds, alert notifications, and dashboard widgets.
3. Add production security hardening and comprehensive automated tests.
4. Add role-specific reporting views for board and grant reporting support.

## Impact Statement [Business]

This system directly supports Safe Future Foundation operations by reducing inventory uncertainty, improving accountability, and enabling faster, safer distribution workflows for families who depend on the organization.
