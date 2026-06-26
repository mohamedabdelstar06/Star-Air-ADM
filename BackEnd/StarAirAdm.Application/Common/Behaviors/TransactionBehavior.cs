namespace StarAirAdm.Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger;
    // Assuming IApplicationDbContext would expose transactions if needed, but EF Core SaveChanges is transactional by default.
    // If explicit transactions are needed, we would inject a transaction provider or DbContext.
    // For now, we will simply log the transaction lifecycle.
    
    public TransactionBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Skip queries from explicit transactions if possible, but MediatR commands usually change state.
        if (requestName.EndsWith("Query"))
        {
            return await next();
        }

        try
        {
            _logger.LogInformation("Beginning transaction for {Name}", requestName);
            
            // In a real implementation with explicit transactions:
            // using var transaction = await _dbContext.BeginTransactionAsync();
            var response = await next();
            // await transaction.CommitAsync();
            
            _logger.LogInformation("Committed transaction for {Name}", requestName);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during transaction for {Name}", requestName);
            throw;
        }
    }
}
