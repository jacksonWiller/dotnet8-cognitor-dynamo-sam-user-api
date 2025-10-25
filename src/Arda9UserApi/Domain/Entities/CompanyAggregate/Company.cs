using Arda9UserApi.Core;
using Catalog.Core.SharedKernel;
using Catalog.Domain.Entities.CompanyAggregate.Events;
using Catalog.Domain.Enums;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Entities.CompanyAggregate;

public class Company : BaseEntity, IAggregateRoot
{
    public CompanyName Name { get; private set; }
    public Slug Slug { get; private set; }
    public CompanyDocument? Document { get; private set; }
    public Email? Email { get; private set; }
    public Phone? Phone { get; private set; }
    public Address? Address { get; private set; }
    public List<string> Tags { get; private set; }
    public CompanyStatus Status { get; private set; }
    public CompanySettings Settings { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    public Company()
    {
        Name = new CompanyName("Default");
        Slug = new Slug("default");
        Tags = new List<string>();
        Settings = new CompanySettings();
        Status = CompanyStatus.ACTIVE;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public Company(
        CompanyName name,
        Slug slug,
        CompanyDocument? document = null,
        Email? email = null,
        Phone? phone = null,
        Address? address = null,
        List<string>? tags = null,
        CompanySettings? settings = null,
        Guid? createdBy = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Slug = slug ?? throw new ArgumentNullException(nameof(slug));
        Document = document;
        Email = email;
        Phone = phone;
        Address = address;
        Tags = ValidateTags(tags ?? new List<string>());
        Settings = settings ?? new CompanySettings();
        Status = CompanyStatus.ACTIVE;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;

        AddDomainEvent(new CompanyCreatedEvent(this));
    }

    public void Update(
        CompanyName name,
        Email? email = null,
        Phone? phone = null,
        Address? address = null,
        List<string>? tags = null,
        Guid? updatedBy = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email;
        Phone = phone;
        Address = address;
        Tags = ValidateTags(tags ?? new List<string>());
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new CompanyUpdatedEvent(this));
    }

    public void UpdateSettings(CompanySettings settings, Guid? updatedBy = null)
    {
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new CompanyUpdatedEvent(this));
    }

    public void ChangeSlug(Slug newSlug, Guid? updatedBy = null)
    {
        if (newSlug == null)
            throw new ArgumentNullException(nameof(newSlug));

        if (Slug.Value == newSlug.Value)
            throw new InvalidOperationException("New slug must be different from current slug");

        var oldSlug = Slug;
        Slug = newSlug;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new CompanySlugChangedEvent(this, oldSlug.Value, newSlug.Value));
    }

    public void Suspend(string reason, Guid? suspendedBy = null)
    {
        if (Status == CompanyStatus.DISABLED)
            throw new InvalidOperationException("Cannot suspend a disabled company");

        Status = CompanyStatus.SUSPENDED;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = suspendedBy;

        AddDomainEvent(new CompanySuspendedEvent(this, reason));
    }

    public void Resume(Guid? resumedBy = null)
    {
        if (Status != CompanyStatus.SUSPENDED)
            throw new InvalidOperationException("Only suspended companies can be resumed");

        Status = CompanyStatus.ACTIVE;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = resumedBy;

        AddDomainEvent(new CompanyResumedEvent(this));
    }

    public void Disable(Guid? disabledBy = null)
    {
        if (Status == CompanyStatus.DISABLED)
            return; // Already disabled

        Status = CompanyStatus.DISABLED;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = disabledBy;

        AddDomainEvent(new CompanyDisabledEvent(this));
    }

    private static List<string> ValidateTags(List<string> tags)
    {
        if (tags.Count > 20)
            throw new ArgumentException("Cannot have more than 20 tags");

        var validated = new List<string>();
        foreach (var tag in tags)
        {
            if (string.IsNullOrWhiteSpace(tag))
                continue;

            var normalized = tag.Trim().ToLowerInvariant();

            if (normalized.Length < 1 || normalized.Length > 32)
                throw new ArgumentException($"Tag must be between 1 and 32 characters: {tag}");

            if (!System.Text.RegularExpressions.Regex.IsMatch(normalized, @"^[a-z0-9\-_]+$"))
                throw new ArgumentException($"Tag must contain only lowercase letters, numbers, hyphens and underscores: {tag}");

            validated.Add(normalized);
        }

        return validated.Distinct().ToList();
    }
}
