using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using HangmanServer.DataAccess;
using HangmanServer.DTOs;

namespace HangmanServer.BusinessLogic
{
    public class CatalogManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public CatalogManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CategoryDTO>> GetCategoriesAsync(string languageCode)
        {
            var categories = await _unitOfWork.Categories.FindAsync(c => c.LanguageCode == languageCode);
            var categoryList = new List<CategoryDTO>();

            foreach (var cat in categories)
            {
                categoryList.Add(new CategoryDTO
                {
                    CategoryId = cat.CategoryID,
                    CategoryName = cat.CategoryName
                });
            }

            return categoryList;
        }

        public async Task<List<WordDTO>> GetWordsByCategoryAsync(int categoryId)
        {
            var words = await _unitOfWork.Words.FindAsync(w => w.CategoryID == categoryId);
            var wordList = new List<WordDTO>();

            foreach (var word in words)
            {
                wordList.Add(new WordDTO
                {
                    WordId = word.WordID,
                    WordText = word.WordText,
                    Description = word.Description,
                    Length = word.Length
                });
            }

            return wordList;
        }
    }
}