import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Navbar from "./Component/Navbar";
import Footer from "./Component/Footer";
import ProductList from "./Component/ProductList";
import Home from "./Component/Home";
import About from "./Component/About";
import Contact from "./Component/Contact";

function App() {
  return (
    <Router>
      <Navbar />
      <Routes>
        <Route path="/" element={<Home />} />           {/* Home Page */}
        <Route path="/products" element={<ProductList />} />  {/* All Products */}
        <Route path="/about" element={<About />} />     {/* About Page */}
        <Route path="/contact" element={<Contact />} /> {/* Contact Page */}
      </Routes>
      <Footer />
    </Router>
  );
}

export default App;
