select *
from blt.Member
where [ResetToken] = @token
and @checkTime < [ResetTokenExpiresOn]
   