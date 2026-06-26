namespace StarAirAdm.Application.Features.Flights.Commands;

public record DeleteFlightCommand(int FlightId) : IRequest<bool>;

public class DeleteFlightCommandHandler : IRequestHandler<DeleteFlightCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteFlightCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteFlightCommand request, CancellationToken cancellationToken)
    {
        var flight = await _context.FlightTrips.FirstOrDefaultAsync(f => f.Id == request.FlightId, cancellationToken);
        if (flight == null) return false;
        
        _context.FlightTrips.Remove(flight);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
