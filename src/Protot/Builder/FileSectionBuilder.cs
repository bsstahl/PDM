using Protot.Entities;

namespace Protot.Builder;

internal class FileSectionBuilder
{
    private readonly FileSection _fileSection = new();

    public FileSection Build() => _fileSection;

    public FileSectionBuilder Create(FileSectionType sectionType)
    {
        this._fileSection.FileSectionType = sectionType;
        return this;
    }
    public FileSectionBuilder AddLine(string line)
    {
        _fileSection.Lines ??= new List<string>();
        if (!string.IsNullOrWhiteSpace(line))
        {
            this._fileSection.Lines.Add(line);
        }

        return this;
    }
}