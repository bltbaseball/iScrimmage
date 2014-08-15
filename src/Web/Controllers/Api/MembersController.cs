using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using iScrimmage.Core.Data;
using iScrimmage.Core.Data.Exceptions;
using iScrimmage.Core.Data.Models;
using iScrimmage.Core.Queries;
using Web.Filters;

namespace Web.Controllers.Api
{
    [WebApiAuthorize]
    public class MembersController : BaseApiController
    {
        public MembersController(IDataContext context) : base(context)
        {
        }

        public IHttpActionResult Get()
        {
            var query = new MembersQuery();

            var members = Context.Execute(query);

            return Ok<IEnumerable<Member>>(members);
        }

        public IHttpActionResult Post([FromBody] Member member)
        {
            try
            {
                if (member == null)
                {
                    throw new ArgumentNullException("member", "The member body is missing.");
                }

                if (member.GuardianId == Guid.Empty)
                {
                    member.GuardianId = null;
                }

                Context.BeginTransaction();

                member = createMember(member);

                Context.CommitTransaction();

                return Ok<Member>(member);
            }
            catch (Exception ex)
            {
                Context.RollbackTransaction();

                return InternalServerError(ex);
            }
        }

        public IHttpActionResult Put([FromBody] Member member)
        {
            try
            {
                if (member == null)
                {
                    throw new ArgumentNullException("member", "The member body is missing.");
                }

                Context.BeginTransaction();

                member = updateMember(member);

                Context.CommitTransaction();

                return Ok<Member>(member);
            }
            catch (Exception ex)
            {
                Context.RollbackTransaction();

                return InternalServerError(ex);
            }
        }

        public IHttpActionResult Delete(Guid id)
        {
            try
            {
                var member = Context.Get<Member>(id);

                if (member == null)
                {
                    throw new RecordNotFoundException<Member>(id.ToString());
                }

                member.Archived = true;
                member.ModifiedBy = CurrentUser.User.Id;
                member.ModifiedOn = DateTime.UtcNow;

                Context.Update<Member>(id, new
                {
                    member.Archived,
                    member.ModifiedBy,
                    member.ModifiedOn
                });

                return Ok<Member>(member);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private Member createMember(Member member)
        {
            member.Id = Guid.NewGuid();
            member.CreatedBy = CurrentUser.User.Id;
            member.ModifiedBy = CurrentUser.User.Id;
            member.CreatedOn = DateTime.UtcNow;
            member.ModifiedOn = DateTime.UtcNow;

            Context.Insert<Member>(member);

            return member;
        }

        private Member updateMember(Member member)
        {
            var original = Context.Get<Member>(member.Id);

            if (original == null)
            {
                throw new RecordNotFoundException<Member>(member.Id.ToString());
            }

            member.ModifiedBy = CurrentUser.User.Id;
            member.ModifiedOn = DateTime.UtcNow;

            Context.Update<Member>(member.Id, new
            {
                member.DateOfBirth,
                member.Email,
                member.EmailVerified,
                member.FirstName,
                member.LastName,
                member.Gender,
                member.GuardianId,
                member.Photo,
                member.LookingForTeam,
                member.ModifiedBy,
                member.ModifiedOn
            });

            return member;
        }
    }
}