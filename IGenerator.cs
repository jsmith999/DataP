using DataPath.Generators;

namespace DataPath {
    interface IGenerator {
        string Path { get; set; }
        string ContainerName { get; set; }
        string Code { get; }
        int StartCol { get; set; }
        void AddValue(int index, string value);
        void Generate(CommonDecl common);
        void Save();
    }
}
