using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Authoring.Guidance.Properties;
using DataAnnotations = System.ComponentModel.DataAnnotations;
using Word = Microsoft.Office.Interop.Word;

namespace NuPattern.Authoring.Guidance
{
    /// <summary>
    /// Processes table of content guidance documents
    /// </summary>
    public class TocGuidanceProcessor : IGuidanceProcessor
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<TocGuidanceProcessor>();

        /// <summary>
        /// File extension for guidance content files
        /// </summary>
        public const string ContentFileExtension = ".mht";
        private const string LegalFileNameExpression = @"[^\w]";
        private const string ContentLinkFormat = "content://{0}/{1}/{2}" + ContentFileExtension;
        private const string TitleDocumentStyleName = "title";
        private const int MaxTopicHeaderLength = 62;
        private const int TrueValue = -1;
        private const int FalseValue = 0;
        private int shapeIndex = 0;
        private string contentLinkVsixId;
        private string contentLinkContentPath;
        private string documentPath;

        /// <summary>
        /// Creates a new instance of the <see cref="TocGuidanceProcessor"/> class.
        /// </summary>
        public TocGuidanceProcessor(string documentPath, string vsixId, string contentPath)
        {
            Guard.NotNull(() => documentPath, documentPath);
            Guard.NotNull(() => vsixId, vsixId);
            Guard.NotNull(() => contentPath, contentPath);

            this.documentPath = documentPath;
            this.contentLinkVsixId = vsixId;
            this.contentLinkContentPath = contentPath.Replace(@"\", @"/");
        }

        /// <summary>
        /// Calculates the workflow.
        /// </summary>
        [CLSCompliant(false)]
        public Queue<INode> CalculateWorkflow()
        {
            Word.Document document = null;
            try
            {
                document = LoadGuidanceDocument(this.documentPath);
                VerifyDocument(document);

                var nodes = new Queue<INode>();

                // Add initial node
                var title = CleanWordRangeText(document.Paragraphs.First.Range.Text);
                nodes.Enqueue(new Initial
                {
                    Name = title,
                });

                // Add remaining nodes.
                Stack<Word.Paragraph> paras = GetNodeHeadings(document);
                CreateWorkflowNodes(nodes, paras, 0);

                // Ensure at least one content node
                var lastNode = nodes.Last() as Initial;
                if (lastNode != null)
                {
                    nodes.Enqueue(new GuidanceAction
                    {
                        Name = Properties.Resources.TocGuidanceProcessor_SampleTopicPlaceholder,
                    });
                }

                //Add final node
                nodes.Enqueue(new Final
                {
                    Name = "ActivityFinal1",
                });

                return nodes;
            }
            catch (COMException ex)
            {
                throw new InvalidOperationException(
                    String.Format(CultureInfo.CurrentCulture, Properties.Resources.TocGuidanceProcessor_ErrorCalculateWorkflow, this.documentPath, ex.Message), ex);
            }
            finally
            {
                CloseDocumentAndQuit(document);
            }
        }

        /// <summary>
        /// Generates individual web archives from source Word document.
        /// </summary>
        /// <returns></returns>
        [CLSCompliant(false)]
        public IEnumerable<string> GenerateWorkflowDocuments(string targetPath)
        {
            Guard.NotNullOrEmpty(() => targetPath, targetPath);

            // Ensure content directory is created
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            Word.Document document = null;
            try
            {
                document = LoadGuidanceDocument(this.documentPath);
                VerifyDocument(document);

                var addedDocs = new List<string>();

                // Create the documents
                CreateGuidanceDocuments(addedDocs, GetParagraphs(document),
                    targetPath, CreateNewGuidanceDocument);

                return addedDocs;
            }
            catch (COMException ex)
            {
                throw new InvalidOperationException(
                    String.Format(CultureInfo.CurrentCulture, Properties.Resources.TocGuidanceProcessor_ErrorGenerateDocuments, this.documentPath, ex.Message), ex);
            }
            finally
            {
                CloseDocumentAndQuit(document);
            }
        }

        /// <summary>
        /// Validates the guidance document for correctness and completeness.
        /// </summary>
        /// <returns></returns>
        [CLSCompliant(false)]
        public IEnumerable<DataAnnotations.ValidationResult> ValidateDocument()
        {
            var errors = new List<DataAnnotations.ValidationResult>();

            Word.Document document = null;
            try
            {
                try
                {
                    document = LoadGuidanceDocument(this.documentPath);

                    VerifyDocument(document);
                }
                catch (FileNotFoundException)
                {
                    AddAndTraceError(errors,
                        Resources.TocGuidanceProcessor_ValidateDocumentNotFound);
                }
                catch (InvalidOperationException)
                {
                    AddAndTraceError(errors,
                        Resources.TocGuidanceProcessor_ValidateDocumentNotFormed);
                }

                if (!errors.Any())
                {
                    var addedDocs = new List<string>();

                    CreateGuidanceDocuments(addedDocs, GetParagraphs(document),
                        Path.GetTempPath(), (addedDocs2, range, pageFullFileName) =>
                        {
                            // Check for duplicate filenames
                            var addedFilename = Path.GetFileNameWithoutExtension(pageFullFileName);
                            if (addedDocs2.Contains(addedFilename))
                            {
                                AddAndTraceError(errors, string.Format(CultureInfo.CurrentCulture,
                                    Resources.TocGuidanceProcessor_ValidateDuplicateTopic, CleanWordRangeText(range.Sentences[1].Text)));
                            }
                            else
                            {
                                addedDocs2.Add(addedFilename);
                            }

                            // Check for lengthy filenames
                            var overflow = addedFilename.Length - MaxTopicHeaderLength;
                            if (overflow > 0)
                            {
                                AddAndTraceError(errors, string.Format(CultureInfo.CurrentCulture,
                                    Resources.TocGuidanceProcessor_ValidateTooLongTopicLength,
                                    CleanWordRangeText(range.Sentences[1].Text), MaxTopicHeaderLength, overflow));
                            }
                        });

                    // Check for broken hyperlinks
                    document.Hyperlinks.Cast<Word.Hyperlink>()
                        .Where(l => !string.IsNullOrEmpty(l.SubAddress) && l.SubAddress.StartsWith("_", StringComparison.OrdinalIgnoreCase))
                        .ToList<Word.Hyperlink>()
                        .ForEach(l =>
                        {
                            var linkText = l.SubAddress;
                            if (document.Bookmarks.Exists(linkText))
                            {
                                var page = document.Bookmarks[linkText];
                                if (page != null)
                                {
                                    // Ensures link is to a document we've shredded.
                                    var range = document.Range(page.Start);
                                    var headingPara = range.Paragraphs[1];
                                    if (IsPageStartParagraph(headingPara))
                                    {
                                        if (IsHeadlineParagraph(headingPara))
                                        {
                                            AddAndTraceError(errors, string.Format(CultureInfo.CurrentCulture,
                                                Resources.TocGuidanceProcessor_ValidateHyperlinkToHeadline, l.TextToDisplay));
                                        }
                                    }
                                    else
                                    {
                                        AddAndTraceError(errors, string.Format(CultureInfo.CurrentCulture,
                                            Resources.TocGuidanceProcessor_ValidateHyperlinkToContent, l.TextToDisplay));
                                    }
                                }
                                else
                                {
                                    AddAndTraceError(errors, string.Format(CultureInfo.CurrentCulture,
                                                Resources.TocGuidanceProcessor_ValidateHyperlinkNotExist, l.TextToDisplay));
                                }
                            }
                            else
                            {
                                AddAndTraceError(errors, string.Format(CultureInfo.CurrentCulture,
                                            Resources.TocGuidanceProcessor_ValidateHyperlinkNotExist, l.TextToDisplay));
                            }
                        });
                }

                return errors;
            }
            catch (COMException ex)
            {
                throw new InvalidOperationException(
                    String.Format(CultureInfo.CurrentCulture, Properties.Resources.TocGuidanceProcessor_ErrorValidateDocument, this.documentPath, ex.Message), ex);
            }
            finally
            {
                CloseDocumentAndQuit(document);
            }
        }

        private static void CloseDocumentAndQuit(Word.Document document)
        {
            try
            {
                if (document != null)
                {
                    var application = document.Application;

                    //Close Document
                    CloseDocument(document);

                    // Close Application
                    if (application.Documents.Count == 0)
                    {
                        QuitWord(application);
                    }
                }
            }
            catch (COMException ex)
            {
                //Ignore error
                tracer.TraceError(ex.Message);
            }
        }

        private static void QuitWord(Word.Application word)
        {
            object missing = Type.Missing;
            object doNotSaveChanges = Word.WdSaveOptions.wdDoNotSaveChanges;
            ((Word._Application)word).Quit(ref doNotSaveChanges, ref missing, ref missing);
            word = null;
        }

        private static void CloseDocument(Word.Document document)
        {
            if (document != null)
            {
                object missing = Type.Missing;
                object doNotSaveChanges = Word.WdSaveOptions.wdDoNotSaveChanges;
                ((Word._Document)document).Close(ref doNotSaveChanges, ref missing, ref missing);
                document = null;
            }
        }

        private static void VerifyDocument(Word.Document document)
        {
            // Ensure more paragraphs than just the main body of the document
            if (document.Paragraphs.Count <= 1)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, Properties.Resources.TocGuidanceProcessor_ErrorNoContentParagraph, document.FullName));
            }
            else
            {
                // Ensure the document starts with a title paragraph
                if (!IsTitleParagraph(document.Paragraphs.First))
                {
                    throw new InvalidOperationException(
                        string.Format(CultureInfo.CurrentCulture, Properties.Resources.TocGuidanceProcessor_ErrorNoTitleParagraph, document.FullName));
                }
            }
        }

        private static Word.Document LoadGuidanceDocument(string documentPath)
        {
            if (!File.Exists(documentPath))
            {
                throw new FileNotFoundException(
                    string.Format(CultureInfo.CurrentCulture, Properties.Resources.TocGuidanceProcessor_ErrorDocumentNotFound, documentPath));
            }

            var word = new Microsoft.Office.Interop.Word.Application();
            try
            {
                return OpenWordDocument(word, documentPath);
            }
            catch (COMException)
            {
                QuitWord(word);
                throw;
            }
        }

        private static Word.Document OpenWordDocument(Word.Application word, string documentPath)
        {
            Word.Document document = null;
            try
            {
                tracer.TraceInformation(
                    Resources.TocGuidanceProcessor_TraceDocumentOpeningAsReadOnly, documentPath);

                // Try open read-only, which will fail if it is a master document.
                document = word.Documents.Open(documentPath, ReadOnly: true);
            }
            catch (COMException)
            {
                tracer.TraceInformation(
                    Resources.TocGuidanceProcessor_TraceDocumentFailedOpenAsReadOnly, documentPath);

                // Remove r/o on disk if any, for master documents that are readon-only on disk
                MakeWritable(documentPath);

                tracer.TraceInformation(
                    Resources.TocGuidanceProcessor_TraceDocumentOpeningAsWritable, documentPath);

                // Try again normally
                document = word.Documents.Open(documentPath);
            }

            if (document == null)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, Properties.Resources.TocGuidanceProcessor_ErrorDocumentOpenFailed, documentPath));
            }

            // Prepare any sub documents of master documents
            if (document.IsMasterDocument)
            {
                tracer.TraceInformation(
                    Resources.TocGuidanceProcessor_TraceOpeningMasterSubdocuments, documentPath);

                // Make all subdocuments readable on disk
                document.Subdocuments.Cast<Word.Subdocument>().ToList()
                    .ForEach(d => MakeWritable(d.Name));

                // Expand all sub-documents for processing
                if (document.Subdocuments.Expanded == false)
                {
                    document.Subdocuments.Expanded = true;
                }
            }

            return document;
        }

        private static void MakeWritable(string documentPath)
        {
            var attributes = File.GetAttributes(documentPath);
            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                tracer.TraceInformation(
                    Resources.TocGuidanceProcessor_TraceDocumentReadOnlyRemoved, documentPath);

                File.SetAttributes(documentPath, attributes & ~FileAttributes.ReadOnly);
            }
        }

        private static void CreateGuidanceDocuments(IList<string> addedDocs, Stack<Word.Paragraph> remainingParagraphs, string targetPath, Action<IList<string>, Word.Range, string> documentProcessor)
        {
            while (remainingParagraphs.Count > 0)
            {
                // Goto next para
                var pageFirstPara = remainingParagraphs.Pop();

                if (IsPageStartParagraph(pageFirstPara))
                {
                    var pageFileName = CleanFileName(pageFirstPara.Range.Text);

                    var pageFullFilePath = Path.Combine(targetPath, pageFileName);
                    var isHeadline = IsHeadlineParagraph(pageFirstPara);

                    // Ensure not last para in document
                    if (remainingParagraphs.Count == 0)
                    {
                        if (!isHeadline)
                        {
                            // Select first (is last) para
                            var pageContent = pageFirstPara.Range;
                            documentProcessor(addedDocs, pageContent, pageFullFilePath);
                        }
                    }
                    else
                    {
                        // Find last para of current page
                        while (remainingParagraphs.Count > 0)
                        {
                            // Check if no content
                            if (IsPageEndParagraph(pageFirstPara))
                            {
                                if (!isHeadline)
                                {
                                    // Select first and only para
                                    var pageContent = pageFirstPara.Application.ActiveDocument.Range(pageFirstPara.Range.Start, pageFirstPara.Range.End);
                                    documentProcessor(addedDocs, pageContent, pageFullFilePath);
                                }
                                break;
                            }
                            else
                            {
                                // Goto next para
                                var pageNextPara = remainingParagraphs.Pop();
                                if (IsPageEndParagraph(pageNextPara))
                                {
                                    if (!isHeadline)
                                    {
                                        // Select from first to last para
                                        var pageContent = pageFirstPara.Application.ActiveDocument.Range(pageFirstPara.Range.Start, pageNextPara.Range.End);
                                        documentProcessor(addedDocs, pageContent, pageFullFilePath);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CreateNewGuidanceDocument(IList<string> addedDocs, Word.Range range, string pageFullFilePath)
        {
            var document = range.Application.ActiveDocument;
            var pageFileName = Path.GetFileNameWithoutExtension(pageFullFilePath);
            var targetPath = Path.GetDirectoryName(pageFullFilePath);
            var savedFilename = Path.ChangeExtension(pageFileName, ContentFileExtension);

            // Create new document
            tracer.TraceInformation(
                Resources.TocGuidanceProcessor_TraceTopicCreated, pageFileName);

            try
            {
                // Copy existing content
                range.Copy();
                PasteNewDocument(targetPath, pageFileName, pageFullFilePath, document);

                if (addedDocs.Contains(savedFilename))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                        Resources.TocGuidanceProcessor_ErrorDuplicateTopic, pageFileName));
                }
                else
                {
                    addedDocs.Add(savedFilename);
                }
            }
            finally
            {
                try
                {
                    // Clear clipboard
                    Clipboard.Clear();
                }
                catch (ThreadStateException)
                {
                }
            }
        }

        private void PasteNewDocument(string targetPath, string pageFileName, string pageFullFilePath, Word.Document document)
        {
            // Create new document, paste, update and save
            object missing = Type.Missing;
            object visible = false;
            var newDocument = document.Application.Documents.Add(ref missing, ref missing, ref missing, ref visible);
            newDocument.CopyStylesFromTemplate(document.FullName);
            try
            {
                newDocument.Range().Paste();

                UpdateHeadingStyles(newDocument);

                UpdateHyperlinks(newDocument, document);

                newDocument.Application.ChangeFileOpenDirectory(targetPath);
                EnsureFileNotExist(pageFullFilePath);
                newDocument.SaveAs(FileName: pageFileName, FileFormat: Word.WdSaveFormat.wdFormatWebArchive, AddToRecentFiles: false);
            }
            finally
            {
                CloseDocument(newDocument);
            }
        }

        private static void EnsureFileNotExist(string filePath)
        {
            if (File.Exists(filePath))
            {
                var file = new FileInfo(filePath);
                if (file.IsReadOnly)
                {
                    file.IsReadOnly = false;
                }
                file.Delete();
            }
        }

        private void UpdateHeadingStyles(Word.Document document)
        {
            //Remove PageBreakBefore from first para where style is heading
            var heading = document.Paragraphs
                .OfType<Word.Paragraph>()
                .Where(p => p.OutlineLevel != Word.WdOutlineLevel.wdOutlineLevelBodyText && p.PageBreakBefore == TrueValue)
                .FirstOrDefault();
            if (heading != null)
            {
                heading.PageBreakBefore = FalseValue;
            }
        }

        private void UpdateHyperlinks(Word.Document targetDocument, Word.Document sourceDocument)
        {
            targetDocument.Hyperlinks.Cast<Word.Hyperlink>()
                .Where(l => !string.IsNullOrEmpty(l.SubAddress) && l.SubAddress.StartsWith("_"))
                .ToList<Word.Hyperlink>()
                .ForEach(l =>
                {
                    var linkText = l.SubAddress;
                    if (sourceDocument.Bookmarks.Exists(linkText))
                    {
                        var page = sourceDocument.Bookmarks[linkText];
                        if (page != null)
                        {
                            // Ensures link is to a document we've shredded.
                            var range = sourceDocument.Range(page.Start);
                            var headingPara = range.Paragraphs[1];
                            if (IsPageStartParagraph(headingPara))
                            {
                                if (!IsHeadlineParagraph(headingPara))
                                {
                                    // Set the new link.
                                    var contentLink = FormatContentLink(headingPara.Range.Text);
                                    l.Delete();
                                    targetDocument.Hyperlinks.Add(Anchor: l.Range, Address: contentLink);
                                }
                                else
                                {
                                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                        Resources.TocGuidanceProcessor_ErrorHyperlinkToHeadline, l.TextToDisplay));
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                    Resources.TocGuidanceProcessor_ErrorHyperlinkToContent, l.TextToDisplay));
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                Resources.TocGuidanceProcessor_ErrorHyperlinkNotExist, l.TextToDisplay));
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                            Resources.TocGuidanceProcessor_ErrorHyperlinkNotExist, l.TextToDisplay));
                    }
                });
        }

        private void CreateWorkflowNodes(Queue<INode> nodes, Stack<Word.Paragraph> headings, int currentLevel)
        {
            // Pull next para from stack
            while (headings.Count > 0)
            {
                // Get next heading
                var para = headings.Pop();

                var level = (int)para.OutlineLevel;
                var isHeadline = IsHeadlineParagraph(para);
                var isTopic = !isHeadline;
                var topicName = CleanWordRangeText(para.Range.Text);
                var topicDescription = GetHeadlineDescription(para);

                if (level > currentLevel)
                {
                    if (isHeadline)
                    {
                        // Add a fork for this new level
                        nodes.Enqueue(new Fork
                        {
                            Name = topicName,
                            Description = topicDescription,
                            //Link = FormatContentLink(topicName),
                        });

                        // Check for any sub-topics
                        if (headings.Count > 0)
                        {
                            Word.Paragraph nextPara = headings.Peek();
                            int nextParaLevel = (int)nextPara.OutlineLevel;
                            if (nextParaLevel > level)
                            {
                                // Add sub-topics
                                CreateWorkflowNodes(nodes, headings, level);
                            }
                        }

                        // Close fork, and add placeholder if no other sub-topics
                        var lastNode = nodes.Last() as Fork;
                        if (lastNode != null)
                        {
                            nodes.Enqueue(new GuidanceAction
                            {
                                Name = string.Format(CultureInfo.CurrentCulture, Properties.Resources.TocGuidanceProcessor_EmptyHeadlineTopicPlaceholder, topicName),
                                Description = topicDescription,
                            });
                        }

                        nodes.Enqueue(new Join
                        {
                            Name = GetNextShapeName<Join>(),
                        });
                    }

                    if (isTopic)
                    {
                        // Check for any sub-topics
                        if (headings.Count > 0)
                        {
                            Word.Paragraph nextPara = headings.Peek();
                            int nextParaLevel = (int)nextPara.OutlineLevel;
                            if (nextParaLevel > level)
                            {
                                // Add a fork for this new level
                                nodes.Enqueue(new Fork
                                {
                                    Name = topicName,
                                    Description = topicDescription,
                                    Link = FormatContentLink(topicName),
                                });

                                // Add sub topics
                                CreateWorkflowNodes(nodes, headings, level);

                                // Close the fork
                                nodes.Enqueue(new Join
                                {
                                    Name = GetNextShapeName<Join>(),
                                });
                            }
                            else
                            {
                                // Add Standalone Action
                                nodes.Enqueue(new GuidanceAction
                                {
                                    Name = topicName,
                                    Description = topicDescription,
                                    Link = FormatContentLink(topicName),
                                });
                            }
                        }
                        else
                        {
                            // Add Standalone Action
                            nodes.Enqueue(new GuidanceAction
                            {
                                Name = topicName,
                                Description = topicDescription,
                                Link = FormatContentLink(topicName),
                            });
                        }
                    }
                }
                else
                {
                    // Found same or higher level paragraph, push it back on stack and exit.
                    headings.Push(para);
                    return;
                }
            }
        }

        private string FormatContentLink(string topicName)
        {
            return string.Format(CultureInfo.CurrentCulture, ContentLinkFormat,
                contentLinkVsixId, contentLinkContentPath, CleanFileName(topicName));
        }

        private static Stack<Word.Paragraph> GetNodeHeadings(Word.Document document)
        {
            const int TrueValue = -1;

            // Only parapgraphs at start of pages, that have a heading level
            var headings = document.Paragraphs.OfType<Word.Paragraph>().Where(
                p => p.OutlineLevel != Word.WdOutlineLevel.wdOutlineLevelBodyText && p.PageBreakBefore == TrueValue);

            //Remove title paragraph
            headings = headings.Where(p => !IsTitleParagraph(p));

            return new Stack<Word.Paragraph>(headings.Reverse().ToArray<Word.Paragraph>());
        }

        private static Stack<Word.Paragraph> GetParagraphs(Word.Document document)
        {
            var paragraphs = document.Paragraphs.OfType<Word.Paragraph>();

            //Remove title paragraph
            paragraphs = paragraphs.Where(p => !IsTitleParagraph(p));

            return new Stack<Word.Paragraph>(paragraphs.Reverse().ToArray<Word.Paragraph>());
        }

        private static bool IsTitleParagraph(Word.Paragraph para)
        {
            var style = para.get_Style() as Word.Style;
            if ((string.Equals(style.NameLocal, TitleDocumentStyleName, StringComparison.OrdinalIgnoreCase))
                && style.BuiltIn == true)
            {
                return true;
            }

            return false;
        }

        private static bool IsPageStartParagraph(Word.Paragraph para)
        {
            return (para.PageBreakBefore == TrueValue
                && para.Range.Sentences != null
                && para.Range.Sentences.Count > 0
                && !string.IsNullOrEmpty(CleanFileName(para.Range.Sentences[1].Text)));
        }

        private static bool IsPageEndParagraph(Word.Paragraph para)
        {
            var nextPara = para.Next();
            if (nextPara == null)
            {
                return true;
            }
            else
            {
                return (IsPageStartParagraph(nextPara));
            }
        }

        private static bool IsHeadlineParagraph(Word.Paragraph para)
        {
            object nextCount = 1;
            var nextPara = para.Next(ref nextCount);
            object followingCount = 2;
            var followingPara = para.Next(ref followingCount);

            if (nextPara == null)
            {
                return true;
            }
            if (followingPara != null)
            {
                return ((nextPara.PageBreakBefore == TrueValue)
                    || ((followingPara.PageBreakBefore == TrueValue) && (IsHeadlineDescriptionParagraph(nextPara))));
            }
            else
            {
                return ((nextPara.PageBreakBefore == TrueValue)
                    || (IsHeadlineDescriptionParagraph(nextPara)));
            }
        }

        private static bool IsHeadlineDescriptionParagraph(Word.Paragraph para)
        {
            return ((para != null) && (para.PageBreakBefore != TrueValue)
                && (para.Range.Sentences.Count == 1));
        }

        private static string GetHeadlineDescription(Word.Paragraph para)
        {
            if (IsHeadlineParagraph(para) && IsHeadlineDescriptionParagraph(para.Next()))
            {
                return CleanWordRangeText(para.Next().Range.Sentences.First.Text);
            }

            return string.Empty;
        }

        private static string CleanWordRangeText(string text)
        {
            return text.Replace("\r", string.Empty);
        }

        private static string CleanFileName(string filename)
        {
            var result = new Regex(LegalFileNameExpression, RegexOptions.IgnoreCase)
                .Replace(filename, string.Empty);

            return CleanWordRangeText(result);
        }

        private string GetNextShapeName<T>() where T : INode
        {
            shapeIndex++;
            return string.Format(CultureInfo.CurrentCulture, "{0}{1}", typeof(T).Name, shapeIndex);
        }

        private static void AddAndTraceError(IList<DataAnnotations.ValidationResult> errors, string message)
        {
            tracer.TraceWarning(
                message);

            errors.Add(new DataAnnotations.ValidationResult(message));
        }
    }
}
