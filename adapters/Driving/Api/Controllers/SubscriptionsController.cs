using application.Dto.Subscriptions;
using application.Ports.Driving.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using application.Common.Authorization;
using application.Ports.Driving.Auth;

namespace api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/subscriptions")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ICreateSubscriptionPlanUseCase _createSubscriptionPlanUseCase;
        private readonly IListSubscriptionPlansUseCase _listSubscriptionPlansUseCase;
        private readonly IGetSubscriptionPlanByIdUseCase _getSubscriptionPlanByIdUseCase;
        private readonly IUpdateSubscriptionPlanUseCase _updateSubscriptionPlanUseCase;
        private readonly ICreateSubscriptionUseCase _createSubscriptionUseCase;
        private readonly IListSubscriptionsUseCase _listSubscriptionsUseCase;
        private readonly IGetSubscriptionByIdUseCase _getSubscriptionByIdUseCase;
        private readonly IGetUserSubscriptionsUseCase _getUserSubscriptionsUseCase;
        private readonly IGetActiveUserSubscriptionUseCase _getActiveUserSubscriptionUseCase;
        private readonly IGetSubscriptionEntitlementsUseCase _getSubscriptionEntitlementsUseCase;
        private readonly IChangeSubscriptionPlanUseCase _changeSubscriptionPlanUseCase;
        private readonly ICancelSubscriptionUseCase _cancelSubscriptionUseCase;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserAuthorizationService _userAuthorizationService;

        public SubscriptionsController(
            ICreateSubscriptionPlanUseCase createSubscriptionPlanUseCase,
            IListSubscriptionPlansUseCase listSubscriptionPlansUseCase,
            IGetSubscriptionPlanByIdUseCase getSubscriptionPlanByIdUseCase,
            IUpdateSubscriptionPlanUseCase updateSubscriptionPlanUseCase,
            ICreateSubscriptionUseCase createSubscriptionUseCase,
            IListSubscriptionsUseCase listSubscriptionsUseCase,
            IGetSubscriptionByIdUseCase getSubscriptionByIdUseCase,
            IGetUserSubscriptionsUseCase getUserSubscriptionsUseCase,
            IGetActiveUserSubscriptionUseCase getActiveUserSubscriptionUseCase,
            IGetSubscriptionEntitlementsUseCase getSubscriptionEntitlementsUseCase,
            IChangeSubscriptionPlanUseCase changeSubscriptionPlanUseCase,
            ICancelSubscriptionUseCase cancelSubscriptionUseCase,
            ICurrentUserService currentUserService,
            IUserAuthorizationService userAuthorizationService)
        {
            _createSubscriptionPlanUseCase = createSubscriptionPlanUseCase;
            _listSubscriptionPlansUseCase = listSubscriptionPlansUseCase;
            _getSubscriptionPlanByIdUseCase = getSubscriptionPlanByIdUseCase;
            _updateSubscriptionPlanUseCase = updateSubscriptionPlanUseCase;
            _createSubscriptionUseCase = createSubscriptionUseCase;
            _listSubscriptionsUseCase = listSubscriptionsUseCase;
            _getSubscriptionByIdUseCase = getSubscriptionByIdUseCase;
            _getUserSubscriptionsUseCase = getUserSubscriptionsUseCase;
            _getActiveUserSubscriptionUseCase = getActiveUserSubscriptionUseCase;
            _getSubscriptionEntitlementsUseCase = getSubscriptionEntitlementsUseCase;
            _changeSubscriptionPlanUseCase = changeSubscriptionPlanUseCase;
            _cancelSubscriptionUseCase = cancelSubscriptionUseCase;
            _currentUserService = currentUserService;
            _userAuthorizationService = userAuthorizationService;
        }

        [HttpGet("plans")]
        public async Task<ActionResult<IReadOnlyList<SubscriptionPlanResponseDto>>> ListPlans([FromQuery] bool activeOnly = true)
        {
            var plans = await _listSubscriptionPlansUseCase.ExecuteAsync(activeOnly);

            var response = plans.Select(plan => new SubscriptionPlanResponseDto
            {
                SubscriptionPlanId = plan.SubscriptionPlanId,
                Name = plan.Name,
                Description = plan.Description,
                Price = plan.Price,
                Currency = plan.Currency,
                BillingInterval = plan.BillingInterval,
                StorageLimitBytes = plan.StorageLimitBytes,
                MaxFileSizeBytes = plan.MaxFileSizeBytes,
                IsActive = plan.IsActive,
                CreatedAt = plan.CreatedAt,
                UpdatedAt = plan.UpdatedAt
            }).ToList();

            return Ok(response);
        }

        [HttpGet("plans/{subscriptionPlanId:guid}")]
        public async Task<ActionResult<SubscriptionPlanResponseDto>> GetPlanById(Guid subscriptionPlanId)
        {
            var plan = await _getSubscriptionPlanByIdUseCase.ExecuteAsync(subscriptionPlanId);
            if (plan == null)
            {
                return NotFound();
            }

            var response = new SubscriptionPlanResponseDto
            {
                SubscriptionPlanId = plan.SubscriptionPlanId,
                Name = plan.Name,
                Description = plan.Description,
                Price = plan.Price,
                Currency = plan.Currency,
                BillingInterval = plan.BillingInterval,
                StorageLimitBytes = plan.StorageLimitBytes,
                MaxFileSizeBytes = plan.MaxFileSizeBytes,
                IsActive = plan.IsActive,
                CreatedAt = plan.CreatedAt,
                UpdatedAt = plan.UpdatedAt
            };

            return Ok(response);
        }

        [HttpPost("plans")]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        public async Task<ActionResult<SubscriptionPlanResponseDto>> CreatePlan(CreateSubscriptionPlanDto dto)
        {
            var plan = await _createSubscriptionPlanUseCase.ExecuteAsync(
                dto.Name,
                dto.Description,
                dto.Price,
                dto.Currency,
                dto.BillingInterval,
                dto.StorageLimitBytes,
                dto.MaxFileSizeBytes,
                dto.IsActive);

            var response = new SubscriptionPlanResponseDto
            {
                SubscriptionPlanId = plan.SubscriptionPlanId,
                Name = plan.Name,
                Description = plan.Description,
                Price = plan.Price,
                Currency = plan.Currency,
                BillingInterval = plan.BillingInterval,
                StorageLimitBytes = plan.StorageLimitBytes,
                MaxFileSizeBytes = plan.MaxFileSizeBytes,
                IsActive = plan.IsActive,
                CreatedAt = plan.CreatedAt,
                UpdatedAt = plan.UpdatedAt
            };

            return CreatedAtAction(nameof(GetPlanById), new { subscriptionPlanId = plan.SubscriptionPlanId }, response);
        }

        [HttpPut("plans/{subscriptionPlanId:guid}")]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        public async Task<ActionResult<SubscriptionPlanResponseDto>> UpdatePlan(Guid subscriptionPlanId, UpdateSubscriptionPlanDto dto)
        {
            var plan = await _updateSubscriptionPlanUseCase.ExecuteAsync(
                subscriptionPlanId,
                dto.Name,
                dto.Description,
                dto.Price,
                dto.Currency,
                dto.BillingInterval,
                dto.StorageLimitBytes,
                dto.MaxFileSizeBytes,
                dto.IsActive);

            if (plan == null)
            {
                return NotFound();
            }

            var response = new SubscriptionPlanResponseDto
            {
                SubscriptionPlanId = plan.SubscriptionPlanId,
                Name = plan.Name,
                Description = plan.Description,
                Price = plan.Price,
                Currency = plan.Currency,
                BillingInterval = plan.BillingInterval,
                StorageLimitBytes = plan.StorageLimitBytes,
                MaxFileSizeBytes = plan.MaxFileSizeBytes,
                IsActive = plan.IsActive,
                CreatedAt = plan.CreatedAt,
                UpdatedAt = plan.UpdatedAt
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<SubscriptionResponseDto>> CreateSubscription(CreateSubscriptionDto dto)
        {
            if (!_userAuthorizationService.IsAdmin() && _currentUserService.UserId != dto.UserId)
            {
                return Forbid();
            }

            var subscription = await _createSubscriptionUseCase.ExecuteAsync(
                dto.UserId,
                dto.SubscriptionPlanId,
                dto.TrialEndsAt,
                dto.AutoRenew,
                dto.ProviderSubscriptionId);

            var response = new SubscriptionResponseDto
            {
                SubscriptionId = subscription.SubscriptionId,
                SubscriptionPlanId = subscription.SubscriptionPlanId,
                PlanName = subscription.SubscriptionPlan.Name,
                Status = subscription.Status,
                StartedAt = subscription.StartedAt,
                CurrentPeriodStart = subscription.CurrentPeriodStart,
                CurrentPeriodEnd = subscription.CurrentPeriodEnd,
                TrialEndsAt = subscription.TrialEndsAt,
                CancelledAt = subscription.CancelledAt,
                EndedAt = subscription.EndedAt,
                AutoRenew = subscription.AutoRenew,
                ProviderSubscriptionId = subscription.ProviderSubscriptionId,
                UserIds = subscription.UserSubscriptions.Select(us => us.UserId).ToList()
            };

            return CreatedAtAction(nameof(GetSubscriptionById), new { subscriptionId = subscription.SubscriptionId }, response);
        }

        [HttpGet]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        public async Task<ActionResult<IReadOnlyList<SubscriptionResponseDto>>> ListSubscriptions()
        {
            var subscriptions = await _listSubscriptionsUseCase.ExecuteAsync();

            var response = subscriptions.Select(subscription => new SubscriptionResponseDto
            {
                SubscriptionId = subscription.SubscriptionId,
                SubscriptionPlanId = subscription.SubscriptionPlanId,
                PlanName = subscription.SubscriptionPlan.Name,
                Status = subscription.Status,
                StartedAt = subscription.StartedAt,
                CurrentPeriodStart = subscription.CurrentPeriodStart,
                CurrentPeriodEnd = subscription.CurrentPeriodEnd,
                TrialEndsAt = subscription.TrialEndsAt,
                CancelledAt = subscription.CancelledAt,
                EndedAt = subscription.EndedAt,
                AutoRenew = subscription.AutoRenew,
                ProviderSubscriptionId = subscription.ProviderSubscriptionId,
                UserIds = subscription.UserSubscriptions.Select(us => us.UserId).ToList()
            }).ToList();

            return Ok(response);
        }

        [HttpGet("{subscriptionId:guid}")]
        public async Task<ActionResult<SubscriptionResponseDto>> GetSubscriptionById(Guid subscriptionId)
        {
            var subscription = await _getSubscriptionByIdUseCase.ExecuteAsync(subscriptionId);
            if (subscription == null)
            {
                return NotFound();
            }

            // allow only admin or users attached to the subscription
            if (!_userAuthorizationService.IsAdmin())
            {
                var currentUserId = _currentUserService.UserId;
                if (currentUserId == null || !subscription.UserSubscriptions.Any(us => us.UserId == currentUserId))
                {
                    return Forbid();
                }
            }

            var response = new SubscriptionResponseDto
            {
                SubscriptionId = subscription.SubscriptionId,
                SubscriptionPlanId = subscription.SubscriptionPlanId,
                PlanName = subscription.SubscriptionPlan.Name,
                Status = subscription.Status,
                StartedAt = subscription.StartedAt,
                CurrentPeriodStart = subscription.CurrentPeriodStart,
                CurrentPeriodEnd = subscription.CurrentPeriodEnd,
                TrialEndsAt = subscription.TrialEndsAt,
                CancelledAt = subscription.CancelledAt,
                EndedAt = subscription.EndedAt,
                AutoRenew = subscription.AutoRenew,
                ProviderSubscriptionId = subscription.ProviderSubscriptionId,
                UserIds = subscription.UserSubscriptions.Select(us => us.UserId).ToList()
            };

            return Ok(response);
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<IReadOnlyList<SubscriptionResponseDto>>> GetSubscriptionsByUserId(Guid userId)
        {
            if (!_userAuthorizationService.IsAdmin() && _currentUserService.UserId != userId)
            {
                return Forbid();
            }

            var subscriptions = await _getUserSubscriptionsUseCase.ExecuteAsync(userId);

            var response = subscriptions.Select(subscription => new SubscriptionResponseDto
            {
                SubscriptionId = subscription.SubscriptionId,
                SubscriptionPlanId = subscription.SubscriptionPlanId,
                PlanName = subscription.SubscriptionPlan.Name,
                Status = subscription.Status,
                StartedAt = subscription.StartedAt,
                CurrentPeriodStart = subscription.CurrentPeriodStart,
                CurrentPeriodEnd = subscription.CurrentPeriodEnd,
                TrialEndsAt = subscription.TrialEndsAt,
                CancelledAt = subscription.CancelledAt,
                EndedAt = subscription.EndedAt,
                AutoRenew = subscription.AutoRenew,
                ProviderSubscriptionId = subscription.ProviderSubscriptionId,
                UserIds = subscription.UserSubscriptions.Select(us => us.UserId).ToList()
            }).ToList();

            return Ok(response);
        }

        [HttpGet("user/{userId:guid}/active")]
        public async Task<ActionResult<SubscriptionResponseDto>> GetActiveSubscription(Guid userId)
        {
            if (!_userAuthorizationService.IsAdmin() && _currentUserService.UserId != userId)
            {
                return Forbid();
            }

            var subscription = await _getActiveUserSubscriptionUseCase.ExecuteAsync(userId);
            if (subscription == null)
            {
                return NotFound();
            }

            var response = new SubscriptionResponseDto
            {
                SubscriptionId = subscription.SubscriptionId,
                SubscriptionPlanId = subscription.SubscriptionPlanId,
                PlanName = subscription.SubscriptionPlan.Name,
                Status = subscription.Status,
                StartedAt = subscription.StartedAt,
                CurrentPeriodStart = subscription.CurrentPeriodStart,
                CurrentPeriodEnd = subscription.CurrentPeriodEnd,
                TrialEndsAt = subscription.TrialEndsAt,
                CancelledAt = subscription.CancelledAt,
                EndedAt = subscription.EndedAt,
                AutoRenew = subscription.AutoRenew,
                ProviderSubscriptionId = subscription.ProviderSubscriptionId,
                UserIds = subscription.UserSubscriptions.Select(us => us.UserId).ToList()
            };

            return Ok(response);
        }

        [HttpGet("user/{userId:guid}/entitlements")]
        public async Task<ActionResult<SubscriptionEntitlementResponseDto>> GetSubscriptionEntitlements(Guid userId)
        {
            if (!_userAuthorizationService.IsAdmin() && _currentUserService.UserId != userId)
            {
                return Forbid();
            }

            var entitlement = await _getSubscriptionEntitlementsUseCase.ExecuteAsync(userId);
            return Ok(entitlement);
        }

        [HttpPut("{subscriptionId:guid}/plan")]
        public async Task<ActionResult<SubscriptionResponseDto>> ChangeSubscriptionPlan(
            Guid subscriptionId,
            ChangeSubscriptionPlanDto dto)
        {
            var subscription = await _changeSubscriptionPlanUseCase.ExecuteAsync(subscriptionId, dto.SubscriptionPlanId);
            if (subscription == null)
            {
                return NotFound();
            }

            if (!_userAuthorizationService.IsAdmin())
            {
                var currentUserId = _currentUserService.UserId;
                if (currentUserId == null || !subscription.UserSubscriptions.Any(us => us.UserId == currentUserId))
                {
                    return Forbid();
                }
            }

            var response = new SubscriptionResponseDto
            {
                SubscriptionId = subscription.SubscriptionId,
                SubscriptionPlanId = subscription.SubscriptionPlanId,
                PlanName = subscription.SubscriptionPlan.Name,
                Status = subscription.Status,
                StartedAt = subscription.StartedAt,
                CurrentPeriodStart = subscription.CurrentPeriodStart,
                CurrentPeriodEnd = subscription.CurrentPeriodEnd,
                TrialEndsAt = subscription.TrialEndsAt,
                CancelledAt = subscription.CancelledAt,
                EndedAt = subscription.EndedAt,
                AutoRenew = subscription.AutoRenew,
                ProviderSubscriptionId = subscription.ProviderSubscriptionId,
                UserIds = subscription.UserSubscriptions.Select(us => us.UserId).ToList()
            };

            return Ok(response);
        }

        [HttpPut("{subscriptionId:guid}/cancel")]
        public async Task<ActionResult<SubscriptionResponseDto>> CancelSubscription(
            Guid subscriptionId,
            [FromQuery] bool cancelImmediately = false)
        {
            var subscription = await _cancelSubscriptionUseCase.ExecuteAsync(subscriptionId, cancelImmediately);
            if (subscription == null)
            {
                return NotFound();
            }

            if (!_userAuthorizationService.IsAdmin())
            {
                var currentUserId = _currentUserService.UserId;
                if (currentUserId == null || !subscription.UserSubscriptions.Any(us => us.UserId == currentUserId))
                {
                    return Forbid();
                }
            }

            var response = new SubscriptionResponseDto
            {
                SubscriptionId = subscription.SubscriptionId,
                SubscriptionPlanId = subscription.SubscriptionPlanId,
                PlanName = subscription.SubscriptionPlan.Name,
                Status = subscription.Status,
                StartedAt = subscription.StartedAt,
                CurrentPeriodStart = subscription.CurrentPeriodStart,
                CurrentPeriodEnd = subscription.CurrentPeriodEnd,
                TrialEndsAt = subscription.TrialEndsAt,
                CancelledAt = subscription.CancelledAt,
                EndedAt = subscription.EndedAt,
                AutoRenew = subscription.AutoRenew,
                ProviderSubscriptionId = subscription.ProviderSubscriptionId,
                UserIds = subscription.UserSubscriptions.Select(us => us.UserId).ToList()
            };

            return Ok(response);
        }
    }
}
