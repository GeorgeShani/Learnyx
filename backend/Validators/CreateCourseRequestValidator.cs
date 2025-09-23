using FluentValidation;
using learnyx.Models.Requests;

namespace learnyx.Validators;

public class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequest>
{
    public CreateCourseRequestValidator()
    {
        // Title is required
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Course title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        // Subtitle optional but limited
        RuleFor(x => x.Subtitle)
            .MaximumLength(300).WithMessage("Subtitle must not exceed 300 characters.")
            .When(x => !string.IsNullOrEmpty(x.Subtitle));

        // Description optional but limited
        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        // Category optional but limited
        RuleFor(x => x.Category)
            .MaximumLength(100).WithMessage("Category must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Category));

        // Level must be valid if provided
        RuleFor(x => x.Level)
            .IsInEnum().When(x => x.Level.HasValue);

        // Language required
        RuleFor(x => x.Language)
            .NotEmpty().WithMessage("Language is required.")
            .MaximumLength(50).WithMessage("Language must not exceed 50 characters.");

        // Price rules (free vs paid)
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0 for paid courses.")
            .When(x => !x.IsFree);

        RuleFor(x => x.Price)
            .Equal(0).WithMessage("Price must be 0 if the course is free.")
            .When(x => x.IsFree);

        // Tags, Requirements, Learning Outcomes validation
        RuleForEach(x => x.Tags)
            .NotEmpty().WithMessage("Tags cannot contain empty values.")
            .MaximumLength(50).WithMessage("Tag cannot exceed 50 characters.");

        RuleForEach(x => x.Requirements)
            .NotEmpty().WithMessage("Requirements cannot contain empty values.")
            .MaximumLength(200).WithMessage("Requirement cannot exceed 200 characters.");

        RuleForEach(x => x.LearningOutcomes)
            .NotEmpty().WithMessage("Learning outcomes cannot contain empty values.")
            .MaximumLength(200).WithMessage("Learning outcome cannot exceed 200 characters.");

        // Settings required
        RuleFor(x => x.Settings)
            .NotNull().WithMessage("Course settings are required.");

        // Nested validation
        RuleForEach(x => x.Sections)
            .SetValidator(new CourseSectionRequestValidator());
    }
}