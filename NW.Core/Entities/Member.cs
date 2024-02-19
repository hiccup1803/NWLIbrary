using System;
using System.Text;
using System.Collections.Generic;
using NW.Core.Entities.BackOffice;


namespace NW.Core.Entities {

    public class Member : Entity<int>
    {
        public Member() { }
        public virtual string Username { get; set; }
        public virtual string Email { get; set; }
        public virtual int StatusType { get; set; }
        public virtual string Password { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Currency { get; set; }
        public virtual string UniqueId { get; set; }
        public virtual string SecondaryUniqueId { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual int? LevelId { get; set; }
        public virtual int? AffiliateTypeId { get; set; }
        public virtual bool IsTestAccount { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime UpdateDate { get; set; }

        public virtual string Host { get; set; }

        public virtual string AffCode { get; set; }
        public virtual string UnformattedUsername { get; set; }
        public virtual int? CashbackTypeId { get; set; }
        public virtual int? CashbackPercentage { get; set; }

        public virtual IEnumerable<MemberDetail> MemberDetails { get; set; }    
        public virtual IList<Game> FavoriteGames { get; set; }
        public virtual Company Company { get; set; }
        public virtual Level Level { get; set; }
        public virtual AffiliateType AffiliateType { get; set; }
        public virtual IList<PowerUser> PowerUsers { get; set; }
        public virtual IList<MemberSegment> MemberSegments { get; set; }

        public virtual IEnumerable<MemberTag> MemberTags { get; set; }
        
    }

    public class MemberWithDetail : Entity<int>
    {
        public virtual string Username { get; set; }
        public virtual string Email { get; set; }
        public virtual int StatusType { get; set; }
        public virtual string Password { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Currency { get; set; }
        public virtual string UniqueId { get; set; }
        public virtual string SecondaryUniqueId { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual int? LevelId { get; set; }
        public virtual int? AffiliateTypeId { get; set; }
        public virtual bool IsTestAccount { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime UpdateDate { get; set; }

        public virtual string Host { get; set; }

        public virtual string AffCode { get; set; }

        public virtual IEnumerable<MemberDetail> MemberDetails { get; set; }
        public virtual IList<Game> FavoriteGames { get; set; }
        public virtual Company Company { get; set; }
        public virtual Level Level { get; set; }
        public virtual AffiliateType AffiliateType { get; set; }
        public virtual IList<PowerUser> PowerUsers { get; set; }
        public virtual IList<MemberSegment> MemberSegments { get; set; }

        public virtual IEnumerable<MemberTag> MemberTags { get; set; }
        public virtual DateTime? BirthDate { get; set; }
        public virtual string Phone { get; set; }
        public virtual int NetEntId { get; set; }
        public virtual string ST { get; set; }
    }
}
