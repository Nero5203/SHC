# Stripe Integration for SHC

## Overview
This document outlines the complete Stripe integration implementation for the SHC payment system under Clean Architecture.

## Architecture

### Clean Architecture Layers

#### 1. **Domain Layer** (`domain/`)
- **No Stripe dependencies** — domain remains provider-agnostic
- `Purchase` entity extended with provider metadata fields:
  - `ProviderCheckoutSessionId`: Stripe Checkout Session ID
  - `ProviderPaymentIntentId`: Stripe Payment Intent ID
- `PurchaseStatus` enum covers all payment states: `Pending`, `Paid`, `Failed`, `Cancelled`, `Refunded`

#### 2. **Application Layer** (`application/`)

##### Ports (Interfaces)

**Driven Port:** `application/Ports/Driven/Payments/IPaymentGatewayService.cs`
- `CreateCheckoutSessionAsync(PaymentCheckoutRequest)` → Creates Stripe Checkout Session
- `ProcessWebhookAsync(payload, signatureHeader)` → Processes Stripe webhook events

**Driving Ports:** `application/Ports/Driving/Payments/`
- `ICreateCheckoutSessionUseCase.cs` — Initiates payment checkout
- `IProcessPaymentWebhookUseCase.cs` — Handles webhook notifications

**DTOs:** `application/Dto/Payments/`
- `CreateCheckoutSessionDto` — Input for checkout endpoint
- `CheckoutSessionResponseDto` — Response with checkout URL

##### Use Cases
- `application/UseCases/Payments/CreateCheckoutSessionUseCase.cs`
  - Fetches purchase
  - Requests Stripe session creation
  - Stores session ID on purchase entity
  - Returns checkout URL to client

- `application/UseCases/Payments/ProcessPaymentWebhookUseCase.cs`
  - Processes Stripe webhook events
  - Updates purchase status based on payment outcome
  - Stores payment intent ID for refund tracking

#### 3. **Infrastructure/Adapters Layer** (`adapters/`)

**Stripe Implementation:** `adapters/Driven/ExternalServices/Payments/StripePaymentGatewayService.cs`
- Implements `IPaymentGatewayService`
- Stripe.net library integration
- Webhook event parsing and mapping to domain status
- Configuration via `appsettings.json` (`Stripe:SecretKey`, `Stripe:WebhookSecret`)

**Persistence:** `adapters/Driven/Persistence/Repositories/Purchases/PurchaseRepository.cs`
- Added `UpdateAsync()` method for provider metadata persistence

#### 4. **API Layer** (`adapters/Driving/Api/`)

**Controllers:**
- `PaymentsController.cs` — POST `/api/purchases/{purchaseId}/checkout`
  - Accepts success/cancel URLs from client
  - Returns Stripe Checkout URL
  
- `WebhookController.cs` — POST `/api/webhooks/stripe`
  - Validates Stripe signature
  - Delegates to webhook use case
  - Returns 200 OK for valid events

**Dependency Injection:** Updated `Program.cs` with:
```csharp
builder.Services.AddScoped<IPaymentGatewayService, StripePaymentGatewayService>();
builder.Services.AddScoped<ICreateCheckoutSessionUseCase, CreateCheckoutSessionUseCase>();
builder.Services.AddScoped<IProcessPaymentWebhookUseCase, ProcessPaymentWebhookUseCase>();
```

## Database Migrations

Extended `Purchase` entity with Stripe metadata:
```bash
dotnet ef migrations add AddStripePaymentMetadataToPurchase \
  --project adapters/adapters.csproj \
  --startup-project adapters/Driving/Api/api.csproj \
  --context ShcDbContext
```

This adds:
- `ProviderCheckoutSessionId` (VARCHAR, nullable)
- `ProviderPaymentIntentId` (VARCHAR, nullable)

## Configuration

### `appsettings.json`

Add Stripe credentials in the appropriate environment configuration:

```json
{
  "Stripe": {
    "SecretKey": "sk_test_...",
    "WebhookSecret": "whsec_..."
  }
}
```

**For Development:** Use Stripe test keys from https://dashboard.stripe.com/test/apikeys

**For Production:** Use live keys and ensure secrets are managed via secure configuration (environment variables, Azure Key Vault, etc.)

## API Endpoints

### 1. Create Checkout Session
```
POST /api/purchases/{purchaseId}/checkout
Content-Type: application/json

{
  "successUrl": "https://yourapp.com/success",
  "cancelUrl": "https://yourapp.com/cancel"
}

Response: 200 OK
{
  "purchaseId": "guid",
  "checkoutSessionId": "cs_...",
  "checkoutUrl": "https://checkout.stripe.com/..."
}
```

### 2. Webhook Endpoint
```
POST /api/webhooks/stripe
Content-Type: application/json
Stripe-Signature: t=timestamp,v1=signature,...

{
  "type": "checkout.session.completed",
  "data": { ... }
}

Response: 200 OK
```

## Stripe Webhook Events Handled

- **`checkout.session.completed`** → Purchase status: `Paid` (if payment succeeds)
- **`checkout.session.expired`** → Purchase status: `Cancelled`
- **`payment_intent.payment_failed`** → Purchase status: `Failed`

## Environment Setup

### Stripe CLI (for local testing)
```bash
stripe login
stripe listen --forward-to localhost:5000/api/webhooks/stripe
```

This outputs a webhook signing secret to use in development `appsettings.json`.

### NuGet Package
- **Stripe.net** (v51.0.0+) — Official Stripe .NET client library

## Boundary Preservation

✅ **Domain:** No Stripe imports — remains provider-agnostic
✅ **Application:** Uses abstract port (`IPaymentGatewayService`) — no Stripe refs
✅ **Infrastructure:** Stripe implementation isolated in `ExternalServices/Payments`
✅ **API:** Controllers delegate to use cases — no payment logic leakage

## Future Enhancements

1. **Subscription Webhooks** — Handle subscription lifecycle events (`customer.subscription.*`)
2. **Refund Processing** — Implement refund use case with Stripe API
3. **Idempotency Keys** — Prevent duplicate charges during retries
4. **Payment Method Tokenization** — Support saved cards via Stripe Customer API
5. **Multi-Currency Support** — Leverage Stripe's currency conversion
6. **Invoice Generation** — Auto-generate invoices from Stripe payment intents

## Notes

- All Stripe API calls are isolated to `StripePaymentGatewayService`
- Webhook verification via signature validation prevents spoofing
- Purchase `ProviderCheckoutSessionId` and `ProviderPaymentIntentId` enable audit trails and refund operations
- Database migration required before deployment to production

