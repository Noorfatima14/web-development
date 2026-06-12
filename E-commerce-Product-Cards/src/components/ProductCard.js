import React, { useState } from "react";

function ProductCard({ product, onDelete, onEdit }) {
  const [isEditing, setIsEditing] = useState(false);
  const [editedTitle, setEditedTitle] = useState(product.title);
  const [editedDescription, setEditedDescription] = useState(product.description);

  const handleSave = () => {
    onEdit(product.id, {
      ...product,
      title: editedTitle,
      description: editedDescription,
    });
    setIsEditing(false);
  };

  return (
    <div className="card m-2" style={{ width: "16rem" }}>
      <img
        src={product.image}
        className="card-img-top"
        alt={product.title}
        style={{ height: "200px", objectFit: "contain" }}
      />
      <div className="card-body">
        {isEditing ? (
          <>
            <input
              className="form-control mb-2"
              value={editedTitle}
              onChange={(e) => setEditedTitle(e.target.value)}
            />
            <textarea
              className="form-control mb-2"
              value={editedDescription}
              onChange={(e) => setEditedDescription(e.target.value)}
            />
            <button className="btn btn-success btn-sm me-2" onClick={handleSave}>
              Save
            </button>
            <button className="btn btn-secondary btn-sm" onClick={() => setIsEditing(false)}>
              Cancel
            </button>
          </>
        ) : (
          <>
            <h5 className="card-title">{product.title}</h5>
            <p className="card-text">{product.description}</p>
            <p className="card-text fw-bold">Price: ${product.price}</p>
            <span className="badge bg-secondary d-block mb-2">{product.category}</span>

            <button className="btn btn-warning btn-sm me-2" onClick={() => setIsEditing(true)}>
              Edit
            </button>
            <button className="btn btn-danger btn-sm" onClick={() => onDelete(product.id)}>
              Delete
            </button>
          </>
        )}
      </div>
    </div>
  );
}

export default ProductCard;
