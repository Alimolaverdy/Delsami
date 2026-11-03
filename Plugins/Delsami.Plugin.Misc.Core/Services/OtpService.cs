using Nop.Core.Caching;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Domain;

namespace Delsami.Plugin.Misc.Core.Services
{
    public class OtpService : IOtpService
    {
        private readonly IRepository<OTP> _repository;
        private readonly IStaticCacheManager _staticCacheManager;


        public OtpService(IRepository<OTP> repository, IStaticCacheManager staticCacheManager)
        {
            _repository = repository;
            _staticCacheManager = staticCacheManager;
        }

        public async Task<OTP> GetOtpByUsernameAsync(string username)
        {
            //return await _staticCacheManager.GetAsync(GetCacheKey(customerId), async () =>
            //{
            return await _repository.Table
                .Where(x => x.Username.Equals(username)
                         && !x.Used)
                .OrderByDescending(x => x.CreateDateUtc)
                .FirstOrDefaultAsync();
            //});
        }

        public async Task Insert(OTP otp)
        {
            await _repository.InsertAsync(otp);

            //await _staticCacheManager.SetAsync(GetCacheKey(otp.Username), otp);
        }

        public async Task Update(OTP otp)
        {
            await _repository.UpdateAsync(otp);

            //await _staticCacheManager.SetAsync(GetCacheKey(otp.Username), otp);
        }

        private CacheKey GetCacheKey(int customerId)
        {
            var cacheKey = new CacheKey("ZarIran.OtpCacheKey.{0}-{1}", customerId.ToString(), "ActivationCode");
            cacheKey.CacheTime = 5;

            return cacheKey;
        }
    }
}