using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Tranning.Validations;

namespace Tranning.Models
{
    public class TopicModel
    {
        public List<TopicDetail> TopicDetailLists { get; set; }
    }

    public class TopicDetail
    {
        public int id { get; set; }

        public int course_id { get; set; }

        [Required(ErrorMessage = "Enter name, please")]
        public string name { get; set; }

        public string? description { get; set; }

        public string? videos { get; set; }

        public string? documents { get; set; }

        public string? attach_file { get; set; }

        [AllowedExtensionFile(new string[] { ".mp4" })]
        [AllowedSizeFile(30 * 1024 * 1024)]
        public IFormFile? videoFile { get; set; }

        [AllowedExtensionFile(new string[] { ".doc", ".docx", ".pdf" })]
        [AllowedSizeFile(30 * 1024 * 1024)]
        public IFormFile? documentsFile { get; set; }

        [AllowedExtensionFile(new string[] { ".rar", ".exe" })]
        [AllowedSizeFile(30 * 1024 * 1024)]
        public IFormFile? attach_fileFile { get; set; }

        [Required(ErrorMessage = "Choose Status, please")]
        public string status { get; set; }

        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }

        public DateTime? deleted_at { get; set; }

        public string? course_name { get; set; }
    }
}
