function Filter({ categories, onFilter, selectedCategory }) {
  return (
    <div className="mb-4 text-center">
      {categories.map((cat, index) => (
        <button
          key={index}
          className={`btn m-1 ${
            selectedCategory === cat ? "btn-dark" : "btn-outline-primary"
          }`}
          onClick={() => onFilter(cat)}
        >
          {cat}
        </button>
      ))}
    </div>
  );
}
export default Filter;