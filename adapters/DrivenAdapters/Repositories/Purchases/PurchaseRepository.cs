using adapters.DrivenAdapters.Data;
using Domain.Entities.Purchases;
using Domain.Entities.Purchases.Enums;
using Microsoft.EntityFrameworkCore;
using ports.DrivenPorts.Purchases;

namespace adapters.DrivenAdapters.Repositories.Purchases
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly ShcDbContext _context;

        public PurchaseRepository(ShcDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Purchase purchase)
        {
            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();
        }

        public async Task<Purchase?> GetByIdAsync(Guid purchaseId)
        {
            return await _context.Purchases
                .FirstOrDefaultAsync(p => p.PurchaseId == purchaseId);
        }

        public async Task<IReadOnlyList<Purchase>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Purchases
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<Invoice?> GetInvoiceByPurchaseIdAsync(Guid purchaseId)
        {
            return await _context.Invoices
                .FirstOrDefaultAsync(i => i.PurchaseId == purchaseId);
        }

        public async Task UpdateStatusAsync(Purchase purchase, PurchaseStatus status)
        {
            purchase.Status = status;
            await _context.SaveChangesAsync();
        }
    }
}
