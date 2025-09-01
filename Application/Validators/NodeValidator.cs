using Domain.DTOs.Node;
using Domain.Enumes.Node;
using FluentValidation;

namespace Application.Validators;

public class NodeValidator : AbstractValidator<NodeUpdateDto>
{
    public NodeValidator()
    {
        RuleFor(x => x.NodeName)
            .MinimumLength(3)
            .WithMessage("must be at least 3 characters long")
            .MaximumLength(100)
            .WithMessage("cannot be longer than 100 characters")
            .When(x => x.NodeName != null);
        
        RuleFor(x => x.SshHost)
            .NotEmpty()
            .WithMessage("must not be empty")
            .MaximumLength(100)
            .WithMessage("must not be longer than 100 characters")
            .When(x => x.SshHost != null);
        
        RuleFor(x => x.SshPort)
            .InclusiveBetween(1, 65535)
            .WithMessage("must be between 1 and 65535")
            .When(x => x.SshPort.HasValue);
        
        RuleFor(x => x.SshUsername)
            .NotEmpty()
            .WithMessage("cannot be empty")
            .MaximumLength(50)
            .WithMessage("must not be longer than 50 characters")
            .When(x => x.SshUsername != null);
        
        RuleFor(x => x.Method)
            .IsInEnum()
            .WithMessage("login was not in the correct format")
            .When(x => x.Method.HasValue);
        
        
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("node status was not in the correct format")
            .When(x => x.Status.HasValue);
        
        RuleFor(x => x.IsAvailableForShow)
            .NotNull()
            .WithMessage("status was not null")
            .When(x => x.IsAvailableForShow.HasValue);
    }
    
    public class NodeCreateValidator : AbstractValidator<NodeCreateDto>
    {
        public NodeCreateValidator()
        {
                RuleFor(x => x.NodeName)
            .NotEmpty()
            .WithMessage("name cannot be empty.")
            .MinimumLength(3)
            .WithMessage("must contain 3 characters.")
            .MaximumLength(100)
            .WithMessage("cannot contain more than 100 characters.");
                
        RuleFor(x => x.SshHost)
            .NotEmpty()
            .WithMessage("connect host cannot be empty.")
            .MaximumLength(255)
            .WithMessage("cannot contain more than 255 characters.");
        
        RuleFor(x => x.SshPort)
            .InclusiveBetween(1, 65535)
            .WithMessage("cannot contain more than 65535 characters and less than 1 characters  ports.");
        
        RuleFor(x => x.SshUsername)
            .NotEmpty()
            .WithMessage("cannot SSH Username be empty")
            .MaximumLength(50)
            .WithMessage("cannot contain more than 50 characters and less than 1 characters.");
        
        RuleFor(x => x.Method)
            .IsInEnum()
            .WithMessage("method is invalid format.")
            .NotNull()
            .WithMessage("method is required.");
        
        
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("node state is invalid.")
            .NotNull()
            .WithMessage("node state is required.");
        
        RuleFor(x => x.IsAvailableForShow)
            .NotNull()
            .WithMessage("node state for show is required.");
        }
    }
}