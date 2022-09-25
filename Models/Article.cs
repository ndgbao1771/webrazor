using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class Article
    {
        public int ID { get; set; }

        [StringLength(255, MinimumLength = 5, ErrorMessage = "{0} phải có độ dài từ {2} đến {1} kí tự")]
        [DisplayName("Tiêu đề")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        public string Title { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Ngày tạo")]
        [Required(ErrorMessage = "Phải chọn {0}")]
        public DateTime PublishDate { get; set; }

        [DisplayName("Nội dung bài viết")]
        public string Content {set; get;}
    }
}