import React, { useState } from "react";
import products from "./data/products";
import ProductCard from "./components/ProductCard";
import Filter from "./components/Filter";
import "bootstrap/dist/css/bootstrap.min.css";

function App() {
  const [category, setCategory] = useState("All");
  const [allProducts, setAllProducts] = useState(products);

  const categories = ["All", ...new Set(products.map(p => p.category))];

  const filteredProducts =
    category === "All"
      ? allProducts
      : allProducts.filter(p => p.category === category);

  const deleteProduct = (id) => {
    setAllProducts(allProducts.filter(p => p.id !== id));
  };

  // ✅ Add this function to handle edits
  const editProduct = (id, updatedData) => {
    setAllProducts(prevProducts =>
      prevProducts.map(product =>
        product.id === id ? { ...product, ...updatedData } : product
      )
    );
  };

  return (
    <div className="container">
      <h2 className="text-center mt-4 mb-4">🛍️ Product Listing</h2>

      <Filter categories={categories} onFilter={setCategory} />

      <div className="d-flex flex-wrap justify-content-center">
        {filteredProducts.map(product => (
          <ProductCard
            key={product.id}
            product={product}
            onDelete={deleteProduct}
            onEdit={editProduct} // ✅ Pass edit function
          />
        ))}
      </div>
    </div>
  );
}

export default App;
