using Hospital.Domain.Entities.Base;
using Hospital.Domain.Enums;

namespace Hospital.Domain.Entities.Staff
{
    public class Administrator : User
    {
        public Administrator() => Role = Role.Admin;
    }
}