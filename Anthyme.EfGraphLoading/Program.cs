using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anthyme.EfGraphLoading
{
    class Program
    {
        private static int fetchSize = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Efficient Entity Framework Graph Loading by Anthyme Caillard");
            Console.WriteLine("Initialize ... It should take a few minutes the first time");
            Initialize();

            for (int i = 1; i <= 50; i += 10)
            {
                SetFetchSize(i);
                Lazy();
                Include();
                MultiQuery();
                if (i == 1) i = 0;
            }

            Console.Read();
        }

        static void Initialize()
        {
            using (var context = new GraphContext())
            {
                //if (context.Database.Exists())
                //{
                //    context.Database.Delete();
                //}

                context.Database.Initialize(false);
            }
        }

        static void SetFetchSize(int size)
        {
            fetchSize = size;
            Console.WriteLine("Number of fetch : " + fetchSize);
        }

        static void Lazy()
        {
            using (new PerfCounter("Lazyload", "End in {0}"))
            using (var context = new GraphContext())
            {
                var subjects = context.Subjects.Take(fetchSize).ToList();
                foreach (var subject in subjects)
                {
                    foreach (var category in subject.Categories)
                    {
                        foreach (var subCategory in category.SubCategories)
                        {
                        }
                    }
                }
            }
        }

        static void Include()
        {
            using (new PerfCounter("Includes", "End in {0}"))
            using (var context = new GraphContext())
            {
                var subjects =  context.Subjects.Include("Categories.SubCategories").Take(fetchSize).ToList();
            }
        }

        static void MultiQuery()
        {
            using (new PerfCounter("MultiQry", "End in {0}"))
            using (var context = new GraphContext())
            using (new NoLazyLoadingScope(context))
            {
                var subjects = context.Subjects.Take(fetchSize).ToList();
                var subjectsIds = subjects.Select(s => s.Id).ToArray();

                var categories =
                    context.Categories.Where(c => subjectsIds.Contains(c.Subject.Id)).ToList();

                var categorieIds = categories.Select(s => s.Id).ToArray();

                var subCategories = context.SubCategories
                    .Where(c => categorieIds.Contains(c.Category.Id)).ToList();

                var categoriesDict = categories.GroupBy(c => c.Subject.Id).ToDictionary(c => c.Key, c => c.ToList());
                var subCategoriesDict = subCategories.GroupBy(c => c.Category.Id).ToDictionary(c => c.Key, c => c.ToList());

                foreach (var subject in subjects)
                {
                    var categoriesCollection = (EntityCollection<Category>)subject.Categories;
                    categoriesCollection.Attach(categoriesDict[subject.Id]);
                }

                foreach (var category in categories)
                {
                    var subCategoriesCollection = (EntityCollection<SubCategory>)category.SubCategories;
                    subCategoriesCollection.Attach(subCategoriesDict[category.Id]);
                }
            }
        }
    }
}
