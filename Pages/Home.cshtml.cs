using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using RazorCoursework.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IO;

namespace RazorCoursework.Pages
{
    public class HomeModel : PageModel
    {
        public List<Review> reviews { get; set; }
        public string userName { get; set; }
        public int reviewsPerPage { get; set; } = 10;
        public string currentTag { get; set; }
        public int currentPage { get; set; }
        public int pagesCount { get; set; }

        public void OnGet(string user, int p)
        {
            userName = user;
            if (IsPageCorrect(p))
                LoadReviews();
            else reviews = new List<Review>();
        }

        private bool IsPageCorrect(int p)
        {
            currentPage = p;
            return p >= 1;
        }

        private void LoadReviews()
        {
            string creatorID;
            using (var context = new ApplicationDbContext(
                   new DbContextOptionsBuilder<ApplicationDbContext>()
                   .UseSqlServer(Startup.Connection)
                   .Options))
            {
                creatorID = context.Users.FirstOrDefault(u => u.UserName == userName)?.Id ?? string.Empty;
            }

            using (var context = new AppContentDbContext(
                   new DbContextOptionsBuilder<AppContentDbContext>()
                   .UseSqlServer(Startup.Connection)
                   .Options))
            {
                reviews = (from t in context.Reviews.Include(r => r.TagRelations).ThenInclude(r => r.Tag)
                             where t.ReviewCreatorID == creatorID
                             orderby t.CreationDate descending
                             select t).ToList();

                double reviewsDividedByPages = reviews.Count() / (double)reviewsPerPage;
                pagesCount = (int)Math.Ceiling(reviewsDividedByPages);

                reviews = reviews
                    .Skip((currentPage - 1) * reviewsPerPage)
                    .Take(reviewsPerPage)
                    .ToList();
            }
        }

        public bool AlreadyLikedReview(string reviewID)
        {
            bool result = true;
            using (var context = new AppContentDbContext(
                   new DbContextOptionsBuilder<AppContentDbContext>()
                   .UseSqlServer(Startup.Connection)
                   .Options))
            {
                string currentuserID = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                result = context.ReviewLikes.Any(like =>
                    like.UserID == currentuserID &&
                    like.ReviewID == reviewID);
            }
            return result;
        }

        public int GetLikesCount(string reviewID)
        {
            int result = 0;
            using (var context = new AppContentDbContext(
                   new DbContextOptionsBuilder<AppContentDbContext>()
                   .UseSqlServer(Startup.Connection)
                   .Options))
            {
                result = context.ReviewLikes.Where(like => like.ReviewID == reviewID).Count();
            }
            return result;
        }

        public int GetCreatorLikesCount(string userID)
        {
            int result = 0;
            using (var context = new AppContentDbContext(
                   new DbContextOptionsBuilder<AppContentDbContext>()
                   .UseSqlServer(Startup.Connection)
                   .Options))
            {
                var creatorReviews = context.Reviews.Where(r => r.ReviewCreatorID == userID);
                result = context.ReviewLikes.Where(like => creatorReviews.Any(r => r.ReviewID == like.ReviewID)).Count();
            }
            return result;
        }

        public async Task<IActionResult> OnPostUserPreferences()
        {
            MemoryStream stream = new MemoryStream();
            await Request.Body.CopyToAsync(stream);
            stream.Position = 0;
            UserPreferences preferences;
            using (StreamReader reader = new StreamReader(stream))
            {
                var data = (await reader.ReadToEndAsync()).Split(';');
                bool isAuthenticated = bool.Parse(data[0]);
                string userID = data[1];
                using (var context = new AppContentDbContext(
                   new DbContextOptionsBuilder<AppContentDbContext>()
                   .UseSqlServer(Startup.Connection)
                   .Options))
                {
                    if (isAuthenticated)
                        preferences = AppContentDbContext.GetUserPreferences(userID);
                    else preferences = new UserPreferences();
                }
            }
            return new JsonResult(preferences.IsDarkTheme + ";" + preferences.IsEnglishVersion);
        }
    }
}
