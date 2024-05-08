using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnitTestEx;

namespace UnitTestProject
{
    /// <summary>
    /// Summary description for FileStorageTest
    /// </summary>
    [TestClass]
    public class FileStorageTest
    {
        public const string MAX_SIZE_EXCEPTION = "DIFFERENT MAX SIZE";
        public const string NULL_FILE_EXCEPTION = "NULL FILE";
        public const string NO_EXPECTED_EXCEPTION_EXCEPTION = "There is no expected exception";

        public const string SPACE_STRING = " ";
        public const string FILE_PATH_STRING = "@D:\\JDK-intellij-downloader-info.txt";
        public const string CONTENT_STRING = "Some text";
        public const string REPEATED_STRING = "AA";
        public const string WRONG_SIZE_CONTENT_STRING = "TEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtext";
        public const string TIC_TOC_TOE_STRING = "tictoctoe.game";

        public const int NEW_SIZE = 5;

        public FileStorage storage = new FileStorage(NEW_SIZE);

        /* ПРОВАЙДЕРЫ */

        static private IEnumerable<object[]> NewFilesData
        {
            get
            {
                return new[]
                {
                    new object[] { new File(REPEATED_STRING, CONTENT_STRING) },
                    new object[] { new File(SPACE_STRING, WRONG_SIZE_CONTENT_STRING) },
                    new object[] { new File(FILE_PATH_STRING, CONTENT_STRING) }
                };
            }
        }

        private static IEnumerable<object[]> FilesForDeleteData
        {
            get
            {
                return new[]
                {
                    new object[] { new File(REPEATED_STRING, CONTENT_STRING), REPEATED_STRING },
                    new object[] { null, TIC_TOC_TOE_STRING }
                };
            }
        }

        private static IEnumerable<object[]> NewExceptionFileData
        {
            get
            {
                return new[]
                {
                    new object[] { new File(REPEATED_STRING, CONTENT_STRING) }
                };
            } 
        }

        /* Тестирование записи файла */
        [TestMethod, DynamicData(nameof(NewFilesData))]
        public void WriteTest(File file)
        {
            try
            {
                Assert.IsTrue(storage.Write(file));
                storage.DeleteAllFiles();
            }
            catch
            {
                Console.WriteLine("This method WriteTest return false");
            }
            
        }

        /* Тестирование записи дублирующегося файла */
        [TestMethod, DynamicData(nameof(NewExceptionFileData))]
        public void WriteExceptionTest(File file)
        {
            bool isException = false;
            try
            {
                storage.Write(file);
                Assert.IsFalse(storage.Write(file));
                storage.DeleteAllFiles();
            }
            catch (FileNameAlreadyExistsException)
            {
                isException = true;
            }
            Assert.IsTrue(isException, NO_EXPECTED_EXCEPTION_EXCEPTION);
        }

        /* Тестирование проверки существования файла */
        [TestMethod, DynamicData(nameof(NewFilesData))]
        public void IsExistsTest(File file)
        {
            String name = file.GetFilename();
            Assert.IsFalse(storage.IsExists(name));
            try
            {
                if (!storage.Write(file))
                {
                    Console.WriteLine(String.Format("This method {0} return false", MethodBase.GetCurrentMethod().Name));
                    return;
                }
            }
            catch (FileNameAlreadyExistsException e)
            {
                Console.WriteLine(String.Format("Exception {0} in method {1}", e.GetBaseException(), MethodBase.GetCurrentMethod().Name));
            }

            Assert.IsTrue(storage.IsExists(name));
            storage.DeleteAllFiles();
        }

        /* Тестирование удаления файла */
        [TestMethod, DynamicData(nameof(FilesForDeleteData))]
        public void DeleteTest(File file, String fileName)
        {
            try
            {
                if (!storage.Write(file))
                {
                    Console.WriteLine(String.Format("This method {0} return false", MethodBase.GetCurrentMethod().Name));
                    return;
                }
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(String.Format("This method {0} get nullable value", MethodBase.GetCurrentMethod().Name));
                return;
            }

            Assert.IsTrue(storage.Delete(fileName));
        }

        /* Тестирование получения файлов */
        [TestMethod]
        public void GetFilesTest()
        {
            foreach (File el in storage.GetFiles())
            {
                Assert.IsNotNull(el);
            }
        }

        // Почти эталонный
        /* Тестирование получения файла */
        [TestMethod, DynamicData(nameof(NewFilesData))]
        public void GetFileTest(File expectedFile)
        {
            if (!storage.Write(expectedFile))
            {
                Console.WriteLine(String.Format("This method {0} return false", MethodBase.GetCurrentMethod().Name));
                return;
            }

            File actualfile = storage.GetFile(expectedFile.GetFilename());
            bool difference = actualfile.GetFilename().Equals(expectedFile.GetFilename()) && actualfile.GetSize().Equals(expectedFile.GetSize());

            Assert.IsTrue(difference, string.Format("There is some differences in {0} and {1} or {2} and {3}", expectedFile.GetFilename(), actualfile.GetFilename(), expectedFile.GetSize(), actualfile.GetSize()));
        }

        // Дополнительно
        [TestMethod]
        public void DeleteFilesFromEmptyStorage()
        {
            try
            {
                Assert.IsTrue(storage.DeleteAllFiles());
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("Method DeleteAllFiles return " + ex.Message);
            }
        }

        [TestMethod]
        public void DeleteFileWhichDoesNotExist()
        {
            try
            {
                Assert.IsTrue(storage.Delete("Sergei"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Method Delete return false");
            }
        }

        [TestMethod]
        public void CreateNewStorageAndMaxSizeLessThan0()
        {
            int maxSize = -1000;
            FileStorage storage = new FileStorage(maxSize);
            File file = new File("FileName", "Content");
            try
            {
                Assert.IsTrue(storage.Write(file));
            }
            catch (Exception ex)
            {
                Console.WriteLine("File didn't write. Max size equals " + maxSize + ". File never write in file storage");
            }
        }
    }
}
