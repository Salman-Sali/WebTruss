using FluentValidation;
using MediatR;
using System.Linq;
using WebTruss.Exceptions;

namespace WebTruss.Validation
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(
                    _validators.Select(v =>
                        v.ValidateAsync(context, cancellationToken)));

                var validationFailures = _validators
                   .Select(async validator => await validator.ValidateAsync(new ValidationContext<TRequest>(request)))
                   .SelectMany(validationResult => validationResult.Result.Errors)
                   .Where(validationFailure => validationFailure != null)
                   .Select(x => x.ErrorMessage)
                   .ToList();

                if (validationFailures.Any())
                    throw new AppException(string.Join(", ", validationFailures), System.Net.HttpStatusCode.BadRequest);
            }
            return await next();
        }
    }

}
