using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using razorweb.Data;
using razorweb.Models;
using Microsoft.Extensions.DependencyInjection;

namespace razorweb.Pages.Blog
{
    public class IndexModel : PageModel
    {
        private readonly razorweb.Data.ArticleContext _context;

        public IndexModel(razorweb.Data.ArticleContext context)
        {
            _context = context;
        }

        // số phần tử hiển thị trên 1 trang
        public const int ITEM_PER_PAGE = 10;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage {get; set;}
        public int countPages {get; set;}

        public IList<Article> Article { get; set; }

        // Chuỗi để tìm kiếm, được binding tự động kể cả là truy 
        // cập get
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }
        public async Task OnGetAsync(string SearchString)
        {
            int totalArticle = await _context.Article.CountAsync();
            countPages = (int)Math.Ceiling((double)totalArticle / ITEM_PER_PAGE);

            if(currentPage < 1)
                currentPage = 1;
            if(currentPage > countPages)
                currentPage = countPages;

            // Truy vấn lấy các Article
            // var articles = from a in  _context.Article select a;
            // if (!string.IsNullOrEmpty(SearchString))
            // {
            //     Console.WriteLine(SearchString);
            //     // Truy vấn lọc các Article mà tiêu đề chứa chuỗi tìm kiếm
            //     articles = articles.Where(article => article.Title.Contains(SearchString));
            // }
            // // Đọc (nạp) Article
            // Article = await articles.ToListAsync();

            //nạp các bài viết theo thứ tự mới nhất
            var qr = (from a in _context.Article
                     orderby a.PublishDate descending
                     select a)
                     .Skip((currentPage - 1) * ITEM_PER_PAGE)
                     .Take(ITEM_PER_PAGE);

            if (!string.IsNullOrEmpty(SearchString))
            {
                Article = qr.Where(a => a.Title.Contains(SearchString)).ToList();

            }
            else
            {
                Article = await qr.ToListAsync();
            }
        }
    }
}