using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using RazorCoursework.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using CG.Web.MegaApiClient;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Dropbox.Api;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Localization;

namespace RazorCoursework.Pages
{
    [Authorize]
    public class CreateUserReviewModel : PageModel
    {
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly IStringLocalizer<CreateUserReviewModel> _localizer;
        public CreateUserReviewModel(
            IWebHostEnvironment appEnvironment, 
            IStringLocalizer<CreateUserReviewModel> localizer)
        {
            _appEnvironment = appEnvironment;
            _localizer = localizer;
        }

        [BindProperty] public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "SubjectNameError")]
            [Display(Name = "SubjectName")]
            public string ReviewSubjectName { get; set; }

            [Required(ErrorMessage = "GenreError")]
            [Display(Name = "Genre")]
            public string ReviewSubjectGenre { get; set; }

            [Required(ErrorMessage = "ReviewTextError")]
            [Display(Name = "ReviewText")]
            [DataType(DataType.MultilineText)]
            public string ReviewText { get; set; }

            [Display(Name = "Tags")]
            public string Tags { get; set; }
        }

        public async Task<IActionResult> OnPostTags()
        {
            using var context = AppContentDbContext.Create();
            string[] result = Array.Empty<string>();
            result = await (from t in context.Tags
                            where t.TagName.StartsWith(Request.Form["term"])
                            select t.TagName)
                            .Take(10)
                            .ToArrayAsync();
            return new JsonResult(new { suggestions = result });
        }

        public async Task<IActionResult> OnPost()
        {
            using var context = AppContentDbContext.Create();
            if (ModelState.IsValid)
            {
                var pictureLinks = await GetPictureLinks();
                var newReview = new Review()
                {
                    ReviewCreatorID = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value,
                    ReviewCreatorName = User.Identity.Name,
                    ReviewText = Input.ReviewText,
                    ReviewSubjectGenre = Input.ReviewSubjectGenre,
                    ReviewSubjectName = Input.ReviewSubjectName,
                    CreationDate = DateTime.Now,
                    TagRelations = new List<UserReviewAndTagRelation>(),
                    AttachedPictureLinks = pictureLinks
                };
                context.Reviews.Add(newReview);

                if (Input.Tags?.Length > 0)
                {
                    foreach (var t in from t in Input.Tags.Split(',')
                                      where t.Length > 0
                                      select t.Trim())
                    {
                        var tag = Regex.Replace(t, @"[ ]{2,}", " ");

                        Tag newTag = context.Tags
                            .Include(t => t.ReviewRelations)
                            .FirstOrDefault(_t => _t.TagName == tag);
                        if (newTag == null)
                        {
                            newTag = new Tag()
                            {
                                TagName = tag,
                                ReviewRelations = new List<UserReviewAndTagRelation>()
                            };
                            context.Tags.Add(newTag);
                        }

                        var rel = new UserReviewAndTagRelation()
                        {
                            Tag = newTag,
                            Review = newReview
                        };
                        newTag.ReviewRelations.Add(rel);
                        newReview.TagRelations.Add(rel);
                        context.ReviewAndTagRelations.Add(rel);
                    }
                }

                context.SaveChanges();
            }

            return RedirectToPage("/Home", new { user = User.Identity.Name, p = 1 });
        }

        private async Task<string> GetPictureLinks()
        {
            if (Request.Form.Files == null || Request.Form.Files.Count == 0)
                return string.Empty;

            var tempDirectory = _appEnvironment.WebRootPath + "/files/";
            if (!Directory.Exists(tempDirectory))
                Directory.CreateDirectory(tempDirectory);

            using var dbx = new DropboxClient(
                "sl.BEFWUM1ofDCiPcKD00AeCYMgduuzDUBlzocDjqOkOdvBH6SBg8llAfj-khPPitEwrEvj4abqZovGlaFvwg1YDNv7uqeAxpYixsBRvBOG22HUe5XAwCQeFSy0ggA1gYHqQ-hQICUfgt0E");
            string pictureLinks = string.Empty;
            foreach (var file in Request.Form.Files)
            {
                string filepath = string.Empty;
                if (file.Length > 0)
                {
                    if (file.Length <= 4096 * 1024)
                    {
                        using (var stream = new FileStream(
                            tempDirectory + Guid.NewGuid() + "_" + file.FileName, FileMode.CreateNew))
                        {
                            file.CopyTo(stream);
                            filepath = stream.Name;
                        }
                        using (var fileStream = System.IO.File.Open(filepath, FileMode.Open))
                        {
                            var uploaded = await dbx.Files.UploadAsync(
                                "/" + Guid.NewGuid() + "_" + file.FileName,
                                body: fileStream);
                            pictureLinks +=
                                (await dbx.Sharing.CreateSharedLinkWithSettingsAsync(uploaded.PathLower))
                                .Url.Replace("dl=0", "raw=1") + ";";
                        }
                        System.IO.File.Delete(filepath);
                    }
                    else
                        ModelState.AddModelError(string.Empty, "��� ������������ ����������� �� ������ ��������� 4 ��.");
                }
            }

            return pictureLinks;
        }
    }
}

[AttributeUsage(
    AttributeTargets.Property |
    AttributeTargets.Field | 
    AttributeTargets.Parameter, 
    AllowMultiple = false)]
public sealed class MaxFilesCountAttribute : ValidationAttribute
{
    private readonly int _maxFilesCount;
    public MaxFilesCountAttribute(int maxFilesCount)
    {
        _maxFilesCount = maxFilesCount;
    }

    public override bool IsValid(object value)
    {
        var file = value as IFormFileCollection;
        return file != null && file.Count <= _maxFilesCount;
    }
}